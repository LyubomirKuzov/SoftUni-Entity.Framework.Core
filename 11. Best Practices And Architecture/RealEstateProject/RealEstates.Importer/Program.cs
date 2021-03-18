using RealEstate.Services;
using RealEstates.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RealEstates.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            ImportJsonFile("properties.json");

            ImportJsonFile("houses.json");
        }

        public static void ImportJsonFile(string fileName)
        {
            var dbContext = new ApplicationDbContext();

            IPropertiesService propertiesService = new PropertiesService(dbContext);

            var properties = JsonSerializer.Deserialize<IEnumerable<PropertyAsJson>>(File.ReadAllText(fileName));

            foreach (var jsonProp in properties)
            {
                propertiesService.Add(jsonProp.District, jsonProp.Floor, jsonProp.TotalFloors, jsonProp.Size,
                    jsonProp.YardSize, jsonProp.Year, jsonProp.Type, jsonProp.BuildingType, jsonProp.Price);

                Console.WriteLine(".");
            }
        }
    }
}
