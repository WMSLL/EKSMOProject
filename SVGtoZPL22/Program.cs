using System;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using Svg;
using Svg.Contrib.Render;
using Svg.Contrib.Render.ZPL;

namespace SVGtoZPL22
{
    class Program
    {
        static void Main(string[] args)
        {

            Print();
        }

      
        static void Print()
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += PrintPageHandler;
            printDoc.Print();
        }

       static void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            //  var file = "C:\\Users\\Kizlik.VV\\Desktop\\Текстовый документ (2).SVG";
            var file = "C:\\Users\\Виктор\\OneDrive\\Рабочий стол\\Текстовый документ (2).SVG";
            var svgDocument = SvgDocument.Open(file);
            var bootstrapper = new DefaultBootstrapper();
            var zplTransformer = bootstrapper.CreateZplTransformer();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var zplRenderer = bootstrapper.CreateZplRenderer(zplTransformer,
                                                             characterSet: CharacterSet.ZebraCodePage850);
            var viewMatrix = bootstrapper.CreateViewMatrix(zplTransformer,
                                                           sourceDpi: 90f,
                                                           destinationDpi: 203f,
                                                           viewRotation: ViewRotation.Normal);
            var zplContainer = zplRenderer.GetTranslation(svgDocument,
                                                          viewMatrix);

            var encoding = zplRenderer.GetEncoding();
            var array = zplContainer.ToByteStream(encoding)
                                    .ToArray();
            string t = string.Empty;
            foreach (var item in array)
            {
                t = t + item;
            }
            Console.WriteLine(t);
            //Замените на e.Graphics.DrawImage или любую другую логику
          //  e.Graphics.DrawString(t, new Font("Arial", 14), Brushes.Black, 0, 0);
        }
    }
}
