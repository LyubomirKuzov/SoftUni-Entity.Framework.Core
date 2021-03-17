using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DTOs
{
    public class ExportSuccessfullySoldProductsUserDTO
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("soldProducts")]
        public List<ExportSuccessfullySoldProductsBuyerDTO> SoldProducts { get; set; } = new List<ExportSuccessfullySoldProductsBuyerDTO>();
    }
}
