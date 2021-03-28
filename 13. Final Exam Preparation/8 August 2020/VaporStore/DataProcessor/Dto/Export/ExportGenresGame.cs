﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VaporStore.DataProcessor.Dto.Export
{
    public class ExportGenresGame
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Developer")]
        public string Developer { get; set; }

        [JsonProperty("Tags")]
        public string Tags { get; set; }

        [JsonProperty("Players")]
        public int Players { get; set; }
    }
}
