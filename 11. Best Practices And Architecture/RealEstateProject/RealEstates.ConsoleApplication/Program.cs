using Microsoft.EntityFrameworkCore;
using RealEstates.Data;
using System;

namespace RealEstates.ConsoleApplication
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            var db = new ApplicationDbContext();
            db.Database.Migrate();

            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1: Property search");
                Console.WriteLine("2: Most expensive districts");
                Console.WriteLine("3: Average price per square meter");
                Console.WriteLine("0: EXIT");

                bool parsed = int.TryParse(Console.ReadLine(), out int option);

                if (parsed && option == 0)
                {
                    break;
                }

                if (parsed && (option >= 1 && option <= 2))
                {
                    switch (option)
                    {
                        case 1:
                            PropertySearch(db);
                            break;

                        case 2:
                            MostExpensiveDistricts(db);
                            break;

                        case 3:
                            AveragePricePerSquareMeter(db);
                            break;

                        default:
                            break;
                    }

                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private static void PropertySearch(ApplicationDbContext db)
        {
            Console.WriteLine("Min price:");
            int minPrice = int.Parse(Console.ReadLine());
            Console.WriteLine("Max price:");
            int maxPrice = int.Parse(Console.ReadLine());
            Console.WriteLine("Min size:");
            int minSize = int.Parse(Console.ReadLine());
            Console.WriteLine("Max size:");
            int maxSize = int.Parse(Console.ReadLine());

            IPropertiesService service = new PropertiesService(db);

            var properties = service.Search(minPrice, maxPrice, minSize, maxSize);

            foreach (var property in properties)
            {
                Console.WriteLine($"{property.DistrictName}; {property.BuildingType}; {property.Type} => {property.Price} EURO => {property.Size}");
            }
        }

        private static void MostExpensiveDistricts(ApplicationDbContext db)
        {
            Console.WriteLine("Districts count:");
            int count = int.Parse(Console.ReadLine());
            IDistrictsService districtsService = new DistrictsService(db);

            var districts = districtsService.GetMostExpensiveDistricts(count);

            foreach (var district in districts)
            {
                Console.WriteLine($"{district.Name} => {district.AveragePricePerSquareMeter:f2} EURO for {district.Size} square meters");
            }
        }

        private static void AveragePricePerSquareMeter(ApplicationDbContext dbContext)
        {
            IPropertiesService propertiesService = new PropertiesService(dbContext);

            Console.WriteLine("Average price per square meter: " + propertiesService.AveragePricePerSquareMeter().ToString("f2") + " EURO");
        }
    }
}
