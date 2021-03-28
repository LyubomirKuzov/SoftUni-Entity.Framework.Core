﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportUsersCard
    {
        [Required]
        [RegularExpression(@"^\d{4} \d{4} \d{4} \d{4}$")]
        [JsonProperty("Number")]
        public string Number { get; set; }

        [Required]
        [RegularExpression(@"^[\d]{3}$")]
        [JsonProperty("CVC")]
        public string Cvc { get; set; }

        [Required]
        [Range(0, 1)]
        [JsonProperty("Type")]
        public CardType Type { get; set; }
    }
}
