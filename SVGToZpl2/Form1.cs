using System.Linq;
using System.Windows.Forms;
using Svg;
using Svg.Contrib.Render;
using Svg.Contrib.Render.ZPL;


namespace SVGToZpl2
{
    public partial class Form1 : Form
    {
        public Form1()
        {           
            InitializeComponent();
         

            var file = "C:\\Users\\Виктор\\OneDrive\\Рабочий стол\\Текстовый документ (2).SVG";
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
            string fff=string.Empty;
            foreach (var item in array)
            {
                fff = fff + item;
            }


        }

    }
}
