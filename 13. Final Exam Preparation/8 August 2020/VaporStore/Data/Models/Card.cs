using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Card
    {
        public Card()
        {
            this.Purchases = new HashSet<Purchase>();
        }



        [Key]
        public int Id { get; set; }

        [Required]
        //Regex validation
        public string Number { get; set; }

        [Required]
        //Regex validation
        public string Cvc { get; set; }

        [Required]
        public CardType Type { get; set; }
        
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}