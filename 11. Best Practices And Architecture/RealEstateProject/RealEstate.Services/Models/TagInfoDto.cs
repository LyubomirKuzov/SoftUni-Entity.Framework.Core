using System.Xml.Serialization;

namespace RealEstate.Services.Models
{
    [XmlType("Tag")]
    public class TagInfoDto
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }
}