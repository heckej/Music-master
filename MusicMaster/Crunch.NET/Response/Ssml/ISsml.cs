using System;
using System.Xml.Linq;
namespace Crunch.NET.Response.Ssml
{
    public interface ISsml
    {
        XNode ToXml();
    }
}
