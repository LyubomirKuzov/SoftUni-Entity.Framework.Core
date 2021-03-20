using PetStore.Data;
using PetStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStore.Services
{
    public interface IAddService
    {
        Pet AddPet(string name, int age, string gender, decimal price, PetType type);

        Product AddProduct(string name, decimal price);

        Buyer AddBuyer(string name);
    }
}
