using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PetStore.Data;
using PetStore.Models;
using PetStore.Services.Models;

namespace PetStore.Core
{
    public class Program
    {
        static void Main(string[] args)
        {
            var db = new PetStoreDbContext();

            db.Database.Migrate();

            while (true)
            {
                StartingUI(db);
            }
        }

        public static void StartingUI(PetStoreDbContext db)
        {
            Console.WriteLine("1) Add pet to store");
            Console.WriteLine("2) Add product to store");
            Console.WriteLine("3) Buy pet");
            Console.WriteLine("4) Buy product");
            Console.WriteLine("5) EXIT");
            Console.Write("Option: ");

            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    AddPet(db);
                    break;

                case 2:
                    AddProduct(db);
                    break;

                case 5:
                default:
                    Console.WriteLine("Leaving the shop...");
                    Environment.Exit(0);
                    break;
            }
        }

        public static void AddPet(PetStoreDbContext db)
        {
            Console.WriteLine("Pet name:");
            string name = Console.ReadLine();
            Console.WriteLine("Pet age:");
            int age = int.Parse(Console.ReadLine());
            Console.WriteLine("Pet gender:");
            string gender = Console.ReadLine();
            Console.WriteLine("Pet price:");
            decimal price = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Pet type:");
            string typeName = Console.ReadLine();

            var type = db.PetTypes.FirstOrDefault(t => t.Name == typeName);

            if (type == null)
            {
                type = new PetType()
                {
                    Name = typeName
                };

                db.PetTypes.Add(type);
            }

            AddService addService = new AddService();

            var pet = addService.AddPet(name, age, gender, price, type);

            db.Pets.Add(pet);

            db.SaveChanges();

            Console.WriteLine("Pet added!");
        }

        public static void AddProduct(PetStoreDbContext db)
        {
            Console.WriteLine("Product name:");
            string name = Console.ReadLine();
            Console.WriteLine("Product price");
            decimal price = decimal.Parse(Console.ReadLine());

            AddService addService = new AddService();

            var product = addService.AddProduct(name, price);

            db.Products.Add(product);

            db.SaveChanges();

            Console.WriteLine("Product added!");
        }

        public static void BuyPet(PetStoreDbContext db)
        {
            Console.WriteLine("Your name is:");
            string buyerName = Console.ReadLine();

            var buyer = db.Buyers.FirstOrDefault(x => x.FirstName == buyerName);

            if (buyer == null)
            {

            }
        }

        public static void BuyProduct(PetStoreDbContext db)
        {

        }
    }
}
