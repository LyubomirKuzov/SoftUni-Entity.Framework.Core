using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Models
{
    public class PetType
    {
        public PetType()
        {
            this.Pets = new HashSet<Pet>();
        }



        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Pet> Pets { get; set; }
    }
}
