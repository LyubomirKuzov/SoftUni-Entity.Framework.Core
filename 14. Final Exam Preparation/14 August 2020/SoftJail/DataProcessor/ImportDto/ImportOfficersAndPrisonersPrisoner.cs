using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Prisoner")]
    public class ImportOfficersAndPrisonersPrisoner
    {
        [Required]
        [XmlAttribute]
        public int id { get; set; }
    }
}
