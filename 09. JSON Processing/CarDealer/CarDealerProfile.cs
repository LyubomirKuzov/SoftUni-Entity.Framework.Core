using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSupplierDTO, Supplier>();

            this.CreateMap<ImportCarDTO, Car>();

            this.CreateMap<ImportCustomerDTO, Customer>();

            this.CreateMap<ImportSaleDTO, Sale>();

            this.CreateMap<Customer, ImportCustomerDTO>()
                .ForMember(x => x.BirthDate, y => y.MapFrom(a => a.BirthDate.ToString("dd/MM/yyyy")));
        }
    }
}
