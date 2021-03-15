using AutoMapper;
using ProductShop.Data;
using ProductShop.DTOs;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserImportDTO, User>();

            this.CreateMap<ProductImportDTO, Product>();

            this.CreateMap<CategoryImportDTO, Category>();

            this.CreateMap<CategoryProductImportDTO, CategoryProduct>();
        }
    }
}
