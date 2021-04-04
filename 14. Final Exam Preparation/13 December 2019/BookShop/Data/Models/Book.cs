using BookShop.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.Data.Models
{
    public class Book
    {
        public Book()
        {
            this.AuthorsBooks = new HashSet<AuthorBook>();
        }



        [Key]
        public int Id { get; set; }

        [Required]
        //MinLength
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        //Validation
        public Genre Genre { get; set; }

        //Range
        public decimal Price { get; set; }

        //Range
        public int Pages { get; set; }

        [Required]
        public DateTime PublishedOn { get; set; }

        public virtual ICollection<AuthorBook> AuthorsBooks { get; set; }
    }
}
