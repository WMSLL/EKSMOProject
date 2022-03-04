using System;
using System.Linq;
using System.Text;
using Svg;


namespace SVGtoZPL22
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = "";
            var svgDocument = SvgDocument.Open(file);
            var bootstrapper = new DefaultBootstrapper();
            var fingerPrintTransformer = bootstrapper.CreateFingerPrintTransformer();
            var fingerPrintRenderer = bootstrapper.CreateFingerPrintRenderer(fingerPrintTransformer);
            var viewMatrix = bootstrapper.CreateViewMatrix(fingerPrintTransformer,
                                                           sourceDpi: 90f,
                                                           destinationDpi: 203f,
                                                           viewRotation: ViewRotation.Normal);
            var fingerPrintContainer = fingerPrintRenderer.GetTranslation(svgDocument,
                                                                          viewMatrix);

            var encoding = fingerPrintRenderer.GetEncoding();
            var array = fingerPrintContainer.ToByteStream(encoding)
                                            .ToArray();

            //var file = "C:\\Users\\Виктор\\OneDrive\\Рабочий стол\\Текстовый документ (2).SVG";
            //var svgDocument = SvgDocument.Open(file);
            //var bootstrapper = new DefaultBootstrapper();
            //var zplTransformer = bootstrapper.CreateZplTransformer();
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //var zplRenderer = bootstrapper.CreateZplRenderer(zplTransformer,
            //                                                 characterSet: CharacterSet.ZebraCodePage850);
            //var viewMatrix = bootstrapper.CreateViewMatrix(zplTransformer,
            //                                               sourceDpi: 90f,
            //                                               destinationDpi: 203f,
            //                                               viewRotation: ViewRotation.Normal);
            //var zplContainer = zplRenderer.GetTranslation(svgDocument,
            //                                              viewMatrix);

            //var encoding = zplRenderer.GetEncoding();
            //var array = zplContainer.ToByteStream(encoding)
            //                        .ToArray();
            //string t = string.Empty;



            //Console.WriteLine(array.ToString());

        }
    }
}
