using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.Export;
using CarDealer.DTO.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AutoMapper.QueryableExtensions;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper = InitializeMapper();

        public static void Main(string[] args)
        {
            var context = new CarDealerContext();

            context.Database.EnsureCreated();

            //var xml = File.ReadAllText("../../../Datasets/sales.xml");

            var xml = GetCarsWithDistance(context);

            GC.Collect();

            Console.WriteLine(xml);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSupplierDTO[]), new XmlRootAttribute("Suppliers"));
            var reader = new StringReader(inputXml);

            var suppliersDTO = (ImportSupplierDTO[])xmlSerializer.Deserialize(reader);

            var suppliers = mapper.Map<Supplier[]>(suppliersDTO);

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPartDTO[]), new XmlRootAttribute("Parts"));

            var reader = new StringReader(inputXml);

            var partsDTO = (ImportPartDTO[])xmlSerializer.Deserialize(reader);

            var parts = mapper.Map<Part[]>(partsDTO)
                .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                .ToArray();

            context.Parts.AddRange(parts);

            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCarDTO[]), new XmlRootAttribute("Cars"));

            var reader = new StringReader(inputXml);

            var carsDTO = (ImportCarDTO[])xmlSerializer.Deserialize(reader);

            var cars = new List<Car>();
            var partCars = new List<PartCar>();

            foreach (var carDTO in carsDTO)
            {
                var car = new Car()
                {
                    Make = carDTO.Make,
                    Model = carDTO.Model,
                    TravelledDistance = carDTO.TravelledDistance
                };

                var carParts = carDTO.Parts
                    .Where(pc => context.Parts.Any(p => p.Id == pc.Id))
                    .Select(pc => pc.Id)
                    .Distinct();

                foreach (var part in carParts)
                {
                    var partCar = new PartCar()
                    {
                        PartId = part,
                        Car = car
                    };

                    partCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.AddRange(partCars);

            context.AddRange(cars);

            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDTO[]), new XmlRootAttribute("Customers"));

            var customersDTO = (ImportCustomerDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = mapper.Map<Customer[]>(customersDTO);

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSaleDTO[]), new XmlRootAttribute("Sales"));

            var salesDTO = (ImportSaleDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var sales = mapper.Map<Sale[]>(salesDTO)
                .Where(x => context.Cars.Any(c => c.Id == x.CarId));

            context.Sales.AddRange(sales);

            context.SaveChanges();

            return $"Successfully imported {sales.Count()}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .Select(c => new ExportCarsWithDistanceCarDTO()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(List<ExportCarsWithDistanceCarDTO>), new XmlRootAttribute("cars"));

            var writer = new StringWriter(sb);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(writer, cars, namespaces);

            return sb.ToString().Trim();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new ExportCarsFromMakeBMW()
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var xmlSerializer = new XmlSerializer(typeof(List<ExportCarsFromMakeBMW>), new XmlRootAttribute("cars"));

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().Trim();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .ProjectTo<ExportLocalSuppliersDTO>(mapper.ConfigurationProvider)
                .ToList();

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var xmlSerializer = new XmlSerializer(typeof(List<ExportLocalSuppliersDTO>), new XmlRootAttribute("suppliers"));

            xmlSerializer.Serialize(new StringWriter(sb), suppliers, namespaces);

            GC.Collect();

            return sb.ToString().Trim();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new CarsWithPartsCarDTO()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(p => new CarsWithPartsPartDTO()
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToList();

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var serializer = new XmlSerializer(typeof(List<CarsWithPartsCarDTO>), new XmlRootAttribute("cars"));

            serializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count() >= 1)
                .Select(c => new ExportTotalSalesCustomerDTO()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToList();

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var serializer = new XmlSerializer(typeof(List<ExportTotalSalesCustomerDTO>), new XmlRootAttribute("customers"));

            serializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new ExportSalesSaleDTO()
                {
                    Car = new ExportSalesCarDTO()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },

                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = s.Car.PartCars.Sum(p => p.Part.Price) -
                                        s.Car.PartCars.Sum(p => p.Part.Price) * s.Discount / 100
                })
                .ToList();

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var serializer = new XmlSerializer(typeof(List<ExportSalesSaleDTO>), new XmlRootAttribute("sales"));

            serializer.Serialize(new StringWriter(sb), sales, namespaces);

            return sb.ToString().Trim();
        }

        public static IMapper InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            var mapper = config.CreateMapper();

            return mapper;
        }
    }
}