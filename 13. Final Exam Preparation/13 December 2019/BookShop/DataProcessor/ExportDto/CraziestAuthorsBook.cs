using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.DataProcessor.ExportDto
{
    public class CraziestAuthorsBook
    {
        [JsonProperty("BookName")]
        public string BookName { get; set; }

        // string???
        [JsonProperty("BookPrice")]
        public string BookPrice { get; set; }
    }
}
