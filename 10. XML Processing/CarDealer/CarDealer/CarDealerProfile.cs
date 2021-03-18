using AutoMapper;
using CarDealer.DTO.Export;
using CarDealer.DTO.Import;
using CarDealer.Models;
using System.Linq;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSupplierDTO, Supplier>();

            this.CreateMap<ImportPartDTO, Part>();

            this.CreateMap<ImportCarDTO, Car>();

            this.CreateMap<ImportCustomerDTO, Customer>();

            this.CreateMap<ImportSaleDTO, Sale>();

            this.CreateMap<Car, ExportCarsWithDistanceCarDTO>();

            this.CreateMap<Car, ExportCarsFromMakeBMW>();

            this.CreateMap<Supplier, ExportLocalSuppliersDTO>();

            this.CreateMap<Part, CarsWithPartsPartDTO>();

            this.CreateMap<Car, CarsWithPartsCarDTO>();

            this.CreateMap<Customer, ExportTotalSalesCustomerDTO>()
                .ForMember(x => x.BoughtCars, y => y.MapFrom(a => a.Sales.Count))
                .ForMember(x => x.SpentMoney, y => y.MapFrom(a => a.Sales.Select(s => s.Car.PartCars.Sum(p => p.Part.Price)).Sum()));

            this.CreateMap<Car, ExportSalesCarDTO>();

            this.CreateMap<Sale, ExportSalesSaleDTO>()
                .ForMember(x => x.Car, y => y.MapFrom(s => s.Car))
                .ForMember(x => x.CustomerName, y => y.MapFrom(s => s.Customer.Name))
                .ForMember(x => x.Discount, y => y.MapFrom(s => s.Discount))
                .ForMember(x => x.Price, y => y.MapFrom(s => s.Car.PartCars.Select(pc => pc.Part.Price).Sum()))
                .ForMember(x => x.PriceWithDiscount,
                        y => y.MapFrom(s => s.Car.PartCars.Select(pc => pc.Part.Price).Sum() -
                                            s.Discount / 100 * s.Car.PartCars.Select(pc => pc.Part.Price).Sum()));
        }
    }
}
