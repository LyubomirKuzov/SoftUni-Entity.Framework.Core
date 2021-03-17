using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs;
using ProductShop.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            //string json = File.ReadAllText("../../../Datasets/categories-products.json");

            using (context)
            {
                InitializeMapper();
                ResetDatabase(context);

                var json = GetUsersWithProducts(context);

                File.WriteAllText("../../../Results/export-users-and-products.json", json);
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var usersDTO = JsonConvert.DeserializeObject<ICollection<UserImportDTO>>(inputJson, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            var users = usersDTO
                .Select(x => Mapper.Map<User>(x))
                .ToList();

            context.Users.AddRange(users);

            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            context.Products.AddRange(products);

            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson)
                .Where(x => x.Name != null)
                .ToList();

            context.Categories.AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson)
                .ToList();

            context.CategoryProducts.AddRange(categoryProducts);

            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .ProjectTo<ExportProductsInRangeDTO>()
                .OrderBy(p => p.Price)
                .ToList();

            var json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count() >= 1 && u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new ExportSuccessfullySoldProductsUserDTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Select(p => new ExportSuccessfullySoldProductsBuyerDTO
                        {
                            Name = p.Name,
                            Price = p.Price,
                            BuyerFirstName = p.Buyer.FirstName,
                            BuyerLastName = p.Buyer.LastName
                        })
                        .ToList()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToList();

            var json = JsonConvert.SerializeObject(users, Formatting.Indented);

            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new ExportCategoryDTO
                {
                    Category = c.Name,
                    ProductsCount = c.CategoryProducts.Select(p => p.Product).Count(),
                    AveragePrice = c.CategoryProducts.Average(p => p.Product.Price).ToString("f2"),
                    TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price).ToString("f2")
                })
                .OrderByDescending(c => c.ProductsCount)
                .ToList();

            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Include(x => x.ProductsSold)
                .ToList()
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Where(p => p.Buyer != null).Count(),
                        products = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Select(p => new
                            {
                                name = p.Name,
                                price = p.Price
                            })
                    }
                })
                .OrderByDescending(u => u.soldProducts.products.Count())
                .ToList();

            var resultObj = new
            {
                usersCount = users.Count(),
                users = users
            };

            var json = JsonConvert.SerializeObject(resultObj, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }

        private static void InitializeMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
        }

        private static void ResetDatabase(ProductShopContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}