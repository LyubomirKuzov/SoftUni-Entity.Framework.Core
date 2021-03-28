using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportUsersUser
    {
        [Required]
        [RegularExpression(@"^[A-Z][a-z]+ [A-Z][a-z]+$")]
        [JsonProperty("FullName")]
        public string FullName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [JsonProperty("Age")]
        [Range(3, 103)]
        public int Age { get; set; }

        [JsonProperty("Cards")]
        public ImportUsersCard[] Cards { get; set; }
    }
}
