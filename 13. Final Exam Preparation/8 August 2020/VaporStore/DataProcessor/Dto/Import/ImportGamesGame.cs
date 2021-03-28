using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportGamesGame
    {
        [Required]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [JsonProperty("ReleaseDate")]
        [Required]
        public string ReleaseDate { get; set; }

        [Required]
        [JsonProperty("Developer")]
        public string Developer { get; set; }

        [Required]
        [JsonProperty("Genre")]
        public string Genre { get; set; }

        [JsonProperty("Tags")]
        public List<string> Tags { get; set; }
    }
}
