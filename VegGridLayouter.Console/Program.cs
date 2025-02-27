using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VegGridLayouter.Core;
using VegGridLayouter.Parser;

namespace VegGridLayouter.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // VegasManager.Instance = new ScriptPortal.Vegas.Vegas()
            VegGrid grid = new VegGrid();
            string xml;


            grid.Margin = "10";
            using (StreamReader sr = new StreamReader("test.xml"))
            {
                
                xml = sr.ReadToEnd();
            }
            // FileStream fs = File.Open("test.xml", FileMode.Open);
            try
            {
                Console.WriteLine("解析XML...");

                grid = VegXmlDeserializer.DeserializeXmlString(xml);

                //int width = grid.CurProject.Video.Width;
                //int height = grid.CurProject.Video.Height;

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
