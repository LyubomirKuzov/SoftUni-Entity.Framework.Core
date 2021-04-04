using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{
    public class ImportAuthor
    {
        [JsonProperty("FirstName")]
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string LastName { get; set; }

        [JsonProperty("Phone")]
        [Required]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$")]
        public string Phone { get; set; }

        [JsonProperty("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [JsonProperty("Books")]
        public List<ImportAuthorBook> Books { get; set; }
    }
}
