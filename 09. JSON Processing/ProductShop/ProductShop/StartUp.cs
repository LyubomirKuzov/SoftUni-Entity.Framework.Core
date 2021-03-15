using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShop.DTOs;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            ResetDatabase(context);

            var json = File.ReadAllText("../../../Datasets/categories-products.json");

            InitializeMapper();

            Console.WriteLine(ImportCategories(context, json));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            var usersDTO = JsonConvert.DeserializeObject<ICollection<UserImportDTO>>(inputJson, settings);

            var users = usersDTO
                .Select(x => Mapper.Map<User>(x))
                .ToList();

            context.Users.AddRange(users);

            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            var productsDTO = JsonConvert.DeserializeObject<ICollection<ProductImportDTO>>(inputJson, settings);

            var products = productsDTO
                .Select(x => Mapper.Map<Product>(x))
                .ToList();

            context.Products.AddRange(products);

            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categoriesDTO = JsonConvert.DeserializeObject<ICollection<CategoryImportDTO>>(inputJson);

            var categories = categoriesDTO
                .Where(x => x.Name != null)
                .Select(x => Mapper.Map<Category>(x))
                .ToList();

            context.Categories.AddRange(categories);

            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProductsDTO = JsonConvert.DeserializeObject<ICollection<CategoryProductImportDTO>>(inputJson);

            var categoryProducts = categoryProductsDTO
                .Select(x => Mapper.Map<CategoryProduct>(x))
                .ToList();

            context.CategoryProducts.AddRange(categoryProducts);

            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
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
            context.Database.EnsureCreated();
            context.Database.EnsureDeleted();
        }
    }
}