using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ExportDto
{
    public class PrisonersCellsOfficersOfficer
    {
        [Required]
        [JsonProperty("OfficerName")]
        public string FullName { get; set; }

        [Required]
        [JsonProperty("Department")]
        public string Department { get; set; }
    }
}
