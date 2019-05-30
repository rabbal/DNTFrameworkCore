using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DNTFrameworkCore.Licensing.Models
{
    public class License
    {
        [XmlAttribute] public Guid Id { set; get; }
        [XmlAttribute] public string IssuedTo { set; get; }
        [XmlAttribute] public string ProductName { set; get; }
        [XmlAttribute] public string Version { get; set; }
        [XmlAttribute] public string SerialNumber { set; get; }
        public List<LicenseFeature> Features { get; set; }
    }
}