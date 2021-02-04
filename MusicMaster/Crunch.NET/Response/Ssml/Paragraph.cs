﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Crunch.NET.Response.Ssml
{
    public class Paragraph : ISsml
    {
        public List<IParagraphSsml> Elements { get; set; } = new List<IParagraphSsml>();

        public Paragraph() { }

        public Paragraph(params IParagraphSsml[] elements)
        {
            Elements = elements.ToList();
        }

        public XNode ToXml()
        {
            return new XElement("p", Elements.Select(e => e.ToXml()));
        }
    }
}
