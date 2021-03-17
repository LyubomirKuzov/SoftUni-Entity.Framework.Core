using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
    public class ImportCustomerDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("birthDate")]
        public string BirthDate { get; set; }

        [JsonProperty("isyoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}
