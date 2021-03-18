using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<ImportUserDTO, User>();

            this.CreateMap<ImportProductDTO, Product>();

            this.CreateMap<ImportCategoryDTO, Category>();

            this.CreateMap<ImportCategoryProductDTO, CategoryProduct>();

            this.CreateMap<Product, ExportProductsInRange>()
                .ForMember(x => x.Name, y => y.MapFrom(a => a.Name))
                .ForMember(x => x.Price, y => y.MapFrom(a => a.Price))
                .ForMember(x => x.Buyer, y => y.MapFrom(a => a.Buyer.FirstName + " " + a.Buyer.LastName));

            this.CreateMap<Category, ExportCategoriesByProductsCountDTO>()
                .ForMember(x => x.Name, y => y.MapFrom(a => a.Name))
                .ForMember(x => x.Count, y => y.MapFrom(a => a.CategoryProducts.Count))
                .ForMember(x => x.AveragePrice, y => y.MapFrom(a => a.CategoryProducts.Average(p => p.Product.Price)))
                .ForMember(x => x.TotalRevenue, y => y.MapFrom(a => a.CategoryProducts.Sum(p => p.Product.Price)));

            this.CreateMap<Product, ExportUsersAndProductsProductDTO>()
                .ForMember(x => x.Name, y => y.MapFrom(a => a.Name))
                .ForMember(x => x.Price, y => y.MapFrom(a => a.Price));
        }
    }
}
