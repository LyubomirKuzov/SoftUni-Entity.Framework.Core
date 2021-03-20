using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Models
{
    public class Buyer
    {
        public Buyer()
        {
            this.Pets = new HashSet<Pet>();
            this.Products = new HashSet<Product>();
        }



        public int Id { get; set; }

        public string FirstName { get; set; }

        public ICollection<Pet> Pets { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
