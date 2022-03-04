using System;
using System.Linq;
using Svg;
using Svg.Contrib.Render;
using Svg.Contrib.Render.ZPL;

namespace CLR_SVGToZPL
{
{
    static class Class1
    {

        public static void Zpl22(string file)
        {
            // var file = "";
            
            var svgDocument = SvgDocument.Open(file);
            var bootstrapper = new DefaultBootstrapper();
            var zplTransformer = bootstrapper.CreateZplTransformer();
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
        }
    }
}
