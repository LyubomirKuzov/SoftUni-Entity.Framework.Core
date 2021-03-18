namespace ProductShop
{
    using AutoMapper;
    using ProductShop.Data;
    using ProductShop.Dtos.Import;
    using ProductShop.Models;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using System;
    using System.Linq;
    using ProductShop.Dtos.Export;
    using System.Text;
    using AutoMapper.QueryableExtensions;

    public class StartUp
    {
        static IMapper mapper = InitializeMapper();

        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            db.Database.EnsureCreated();

            InitializeMapper();

            //var xml = File.ReadAllText("../../../Datasets/categories.xml");

            var xml = GetProductsInRange(db);

            Console.WriteLine(xml);
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(List<ImportUserDTO>), new XmlRootAttribute("Users"));
            var reader = new StringReader(inputXml);

            using (reader)
            {
                var usersDTO = (List<ImportUserDTO>)xmlSerializer.Deserialize(reader);

                var users = Mapper.Map<List<User>>(usersDTO);

                context.Users.AddRange(users);

                context.SaveChanges();

                return $"Successfully imported {users.Count}";
            }
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            //var configuration = MapperConfiguration();
            //var mapper = configuration.CreateMapper();

            var xmlSerializer = new XmlSerializer(typeof(List<ImportProductDTO>), new XmlRootAttribute("Products"));

            var reader = new StringReader(inputXml);

            using (reader)
            {
                var productsDTO = (List<ImportProductDTO>)xmlSerializer.Deserialize(reader);

                var products = mapper.Map<List<Product>>(productsDTO);

                context.Products.AddRange(products);

                context.SaveChanges();

                return $"Successfully imported {products.Count}";
            }
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            //var config = MapperConfiguration();
            //var mapper = config.CreateMapper();

            var xmlSerializer = new XmlSerializer(typeof(List<ImportCategoryDTO>), new XmlRootAttribute("Categories"));
            var reader = new StringReader(inputXml);

            var categoriesDTO = (List<ImportCategoryDTO>)xmlSerializer.Deserialize(reader);

            var categories = mapper.Map<List<Category>>(categoriesDTO);

            context.Categories.AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            //var config = MapperConfiguration();
            //var mapper = config.CreateMapper();

            var xmlSerializer = new XmlSerializer(typeof(List<ImportCategoryProductDTO>), new XmlRootAttribute("CategoryProducts"));
            var reader = new StringReader(inputXml);

            var categoryProductsDTO = (List<ImportCategoryProductDTO>)xmlSerializer.Deserialize(reader);

            var categoryProducts = mapper.Map<List<CategoryProduct>>(categoryProductsDTO)
                .Where(x => context.Categories.Any(c => c.Id == x.CategoryId))
                .Where(x => context.Products.Any(p => p.Id == x.ProductId))
                .ToList();

            context.CategoryProducts.AddRange(categoryProducts);

            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ExportProductsInRange
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .Take(10)
                .ToList();

            var xmlSerializer = new XmlSerializer(typeof(List<ExportProductsInRange>), new XmlRootAttribute("Products"));

            var writer = new StringWriter(sb);

            using (writer)
            {
                xmlSerializer.Serialize(writer, products, namespaces);
            }

            return sb.ToString().Trim();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .Select(u => new ExportGetSoldProductsUserDTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new ExportGetSoldProductsProductDTO()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToArray()
                })
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var xmlSerializer = new XmlSerializer(typeof(ExportGetSoldProductsUserDTO[]), new XmlRootAttribute("Users"));

            var writer = new StringWriter(sb);

            using (writer)
            {
                xmlSerializer.Serialize(writer, users, namespaces);
            }

            return sb.ToString().Trim();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new ExportCategoriesByProductsCountDTO()
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count(),
                    AveragePrice = c.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToList();

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var xmlSerializer = new XmlSerializer(typeof(List<ExportCategoriesByProductsCountDTO>), new XmlRootAttribute("Categories"));

            var writer = new StringWriter(sb);

            using (writer)
            {
                xmlSerializer.Serialize(writer, categories, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = new ExportUsersAndProductsUsersDTO()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any(p => p.Buyer != null)),
                Users = context.Users
                    .ToArray()
                    .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                    .OrderByDescending(u => u.ProductsSold.Count)
                    .Take(10)
                    .Select(u => new ExportUsersAndProductsUserDTO()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProducts = new ExportUsersAndProductsSoldProductsDTO()
                        {
                            Count = u.ProductsSold.Count(p => p.Buyer != null),
                            Products = u.ProductsSold
                                .Where(p => p.Buyer != null)
                                .Select(p => new ExportUsersAndProductsProductDTO()
                                {
                                    Name = p.Name,
                                    Price = p.Price
                                })
                                .OrderByDescending(p => p.Price)
                                .ToArray()
                        }
                    })
                    .ToArray()
            };

            StringBuilder sb = new StringBuilder();

            var writer = new StringWriter(sb);

            var xmlSerializer = new XmlSerializer(typeof(ExportUsersAndProductsUsersDTO), new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(writer, users, namespaces);

            return sb.ToString().Trim();
        }

        public static IMapper InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = config.CreateMapper();

            return mapper;
        }
    }
}