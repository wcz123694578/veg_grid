using System.IO;
using System.Xml.Serialization;
using VegGridLayouter.Core;
using ScriptPortal.Vegas;
using System.Text;

namespace VegGridLayouter.Parser
{
    public class VegXmlDeserializer
    {
        public static string SerializeXml(VegGrid grid)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xz = new XmlSerializer(grid.GetType());
                xz.Serialize(sw, grid);
                return sw.ToString();
            }
        }

        public static VegGrid DeserializeXml(FileStream fs)
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                XmlSerializer xz = new XmlSerializer(typeof(VegGrid));
                VegGrid grid = (VegGrid)xz.Deserialize(sr);
                return grid;
            }
        }
    }
}
