using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
    public class ImportSupplierDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("isImporter")]
        public bool IsImporter { get; set; }
    }
}
