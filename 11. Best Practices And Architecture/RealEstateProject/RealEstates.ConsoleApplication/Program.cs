using Microsoft.EntityFrameworkCore;
using RealEstates.Data;
using System;

namespace RealEstates.ConsoleApplication
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            var db = new ApplicationDbContext();
            db.Database.Migrate();

            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1: Property search");
                Console.WriteLine("2: Most expensive districts");
                Console.WriteLine("3: Average price per square meter");
                Console.WriteLine("4: Add tag");
                Console.WriteLine("5: Bulk tag to properties");
                Console.WriteLine("6: Get properties full info");
                Console.WriteLine("0: EXIT");

                bool parsed = int.TryParse(Console.ReadLine(), out int option);

                if (parsed && option == 0)
                {
                    break;
                }

                if (parsed && (option >= 1 && option <= 6))
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

                        case 4:
                            AddTag(db);
                            break;

                        case 5:
                            BulkTagToProperties(db);
                            break;

                        case 6:
                            GetPropertyFullInfo(db);
                            break;

                        default:
                            break;
                    }

                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private static void GetPropertyFullInfo(ApplicationDbContext db)
        {
            Console.WriteLine("Properties count:");

            int count = int.Parse(Console.ReadLine());

            IPropertiesService propertiesService = new PropertiesService(db);
            var result = propertiesService.GetFullData(count).ToArray();

            var xmlSerializer = new XmlSerializer(typeof(PropertyInfoFullDataDTO[]));

            var writer = new StringWriter();

            var result = xmlSerializer.Serialize(writer, result);

            Console.WriteLine(result.ToString().TrimEnd());

            foreach (var item in result)
            {
                Console.WriteLine(item.DistrictName);
                Console.WriteLine(item.BuildingType);
                Console.WriteLine(item.Id);
                Console.WriteLine(item.Price);
                Console.WriteLine(item.PropertyType);
                Console.WriteLine(item.Size);
                Console.WriteLine(item.Year);
                Console.WriteLine(item.Tags);

                foreach (var tag in item.Tags)
                {
                    Console.WriteLine(tag.Name);
                }
            }
        }

        private static void BulkTagToProperties(ApplicationDbContext db)
        {
            Console.WriteLine("Bulk operation started!");

            IPropertiesService propertiesService = new PropertiesService(db);
            ITagService tagService = new TagService(db, propertiesService);

            tagService.BulkTagToProperties();

            Console.WriteLine("Bulk operation finished!");
        }

        private static void AddTag(ApplicationDbContext db)
        {
            Console.WriteLine("Tag name:");
            string tagName = Console.ReadLine();

            Console.WriteLine("Importance (optional):");

            bool isParsed = int.TryParse(Console.ReadLine(), out int tagImportance);

            IPropertiesService propertiesService = new PropertiesService(db);
            ITagService tagService = new TagService(db, propertiesService);

            int? importance = isParsed ? tagImportance : null;

            tagService.Add(tagName, importance);
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
