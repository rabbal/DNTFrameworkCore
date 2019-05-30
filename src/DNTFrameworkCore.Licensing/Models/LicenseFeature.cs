using System.Xml.Serialization;

namespace DNTFrameworkCore.Licensing.Models
{
    public class LicenseFeature
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string DisplayName { get; set; }
        [XmlAttribute] public string Value { get; set; }
        public string Description { get; set; }
    }
}
