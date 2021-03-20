using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStore.Models
{
    public class Product
    {
        public Product()
        {
            this.Buyers = new HashSet<Buyer>();
        }



        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<Buyer> Buyers { get; set; }
    }
}
