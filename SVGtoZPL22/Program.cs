﻿using System;
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

            var tttt =svgDocument.Draw();
            svgDocument.Draw(tttt );



        }
    }
}
