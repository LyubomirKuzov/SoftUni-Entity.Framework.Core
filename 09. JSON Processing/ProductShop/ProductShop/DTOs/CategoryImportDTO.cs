using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DTOs
{
    public class CategoryImportDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
