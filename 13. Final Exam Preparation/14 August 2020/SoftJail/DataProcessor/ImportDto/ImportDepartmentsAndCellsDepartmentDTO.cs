using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportDepartmentsAndCellsDepartmentDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("Cells")]
        public ImportDepartmentsAndCellsCellDTO[] Cells { get; set; }
    }
}
