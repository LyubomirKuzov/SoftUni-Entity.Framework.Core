using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();

            context.Database.EnsureCreated();

            using (context)
            {
                InitializeMapper();

                //var json = JsonConvert.SerializeObject(File.ReadAllText("../../../Datasets/sales.json"));

                Console.WriteLine(GetSalesWithAppliedDiscount(context));
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliersDTO = JsonConvert.DeserializeObject<List<ImportSupplierDTO>>(inputJson);

            var suppliers = suppliersDTO
                .Select(x => Mapper.Map<Supplier>(x))
                .ToList();

            context.Suppliers.AddRange(suppliers);

            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
            .Where(x => Enumerable.Range(context.Suppliers.Min(s => s.Id), context.Suppliers.Max(s => s.Id))
            .Contains(x.SupplierId))
            .ToList();

            context.Parts.AddRange(parts);

            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsDTO = JsonConvert.DeserializeObject<List<ImportCarDTO>>(inputJson);

            var cars = new List<Car>();

            foreach (var carDto in carsDTO)
            {
                var car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };

                foreach (var partId in carDto.PartsId.Distinct())
                {
                    car.PartCars.Add(new PartCar()
                    {
                        Car = car,
                        PartId = partId
                    });
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);

            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customersDTO = JsonConvert.DeserializeObject<List<ImportCustomerDTO>>(inputJson);

            var customers = customersDTO
                .Select(x => Mapper.Map<Customer>(x))
                .ToList();

            context.Customers.AddRange(customers);

            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var salesDTO = JsonConvert.DeserializeObject<List<ImportSaleDTO>>(inputJson);

            var sales = salesDTO
                .Select(x => Mapper.Map<Sale>(x))
                .ToList();

            context.Sales.AddRange(sales);

            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();


            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToList();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return json;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                    },

                    parts = c.PartCars.Select(p => new
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price.ToString("f2")
                    })
                    .ToList()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count() >= 1)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToList();

            var json = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return json;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },

                    customerName = s.Customer.Name,
                    Discount = s.Discount.ToString("f2"),
                    price = s.Car.PartCars.Sum(p => p.Part.Price).ToString("f2"),
                    priceWithDiscount = (s.Car.PartCars.Sum(p => p.Part.Price) * (1 - s.Discount / 100)).ToString("f2")
                })
                .ToList();

            var json = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return json;
        }

        public static void InitializeMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });
        }
    }
}