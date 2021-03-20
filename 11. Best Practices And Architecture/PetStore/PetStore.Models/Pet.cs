using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Models
{
    public class Pet
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Gender { get; set; }

        public decimal Price { get; set; }

        public bool IsSold { get; set; }

        public int PetTypeId { get; set; }

        public virtual PetType PetType { get; set; }

        public int? BuyerId { get; set; }

        public virtual Buyer Buyer { get; set; }
    }
}
