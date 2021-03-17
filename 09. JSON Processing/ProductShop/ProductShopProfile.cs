using AutoMapper;
using ProductShop.DTOs;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserImportDTO, User>();

            this.CreateMap<Product, ExportProductsInRangeDTO>()
                .ForMember(x => x.Seller, y => y.MapFrom(p => p.Seller.FirstName + " " + p.Seller.LastName));            
        }
    }
}
