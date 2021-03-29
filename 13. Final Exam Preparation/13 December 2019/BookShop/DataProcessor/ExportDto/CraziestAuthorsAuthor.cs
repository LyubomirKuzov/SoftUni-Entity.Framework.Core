using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.DataProcessor.ExportDto
{
    public class CraziestAuthorsAuthor
    {
        [JsonProperty("AuthorName")]
        public string AuthorName { get; set; }

        [JsonProperty("Books")]
        public List<CraziestAuthorsBook> Books { get; set; }
    }
}
