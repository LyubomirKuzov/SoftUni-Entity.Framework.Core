using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class User
    {
        public User()
        {
            this.Cards = new HashSet<Card>();
        }



        [Key]
        public int Id { get; set; }

        [Required]
        //MinLength
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        //Regex validation
        public string FullName { get; set; }

        [Required]
        //Validation ???
        public string Email { get; set; }

        [Required]
        //Range validation
        public int Age { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
    }
}
