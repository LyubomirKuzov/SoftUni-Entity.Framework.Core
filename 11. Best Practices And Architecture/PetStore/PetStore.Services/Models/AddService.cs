using PetStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStore.Services.Models
{
    public class AddService : IAddService
    {
        public Buyer AddBuyer(string name)
        {
            var buyer = new Buyer()
            {
                FirstName = name
            };

            return buyer;
        }

        public Pet AddPet(string name, int age, string gender, decimal price, PetType type)
        {
            var pet = new Pet()
            {
                Name = name,
                Age = age,
                Gender = gender,
                Price = price,
                PetType = type,
                IsSold = false,
                Buyer = null
            };

            return pet;
        }

        public Product AddProduct(string name, decimal price)
        {
            var product = new Product()
            {
                Name = name,
                Price = price
            };

            return product;
        }
    }
}
