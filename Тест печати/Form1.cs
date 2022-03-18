using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Тест_печати
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // PrintFile("C:\\Users\\Виктор\\OneDrive\\Рабочий стол\\Текстовый документ (2).svg");
           // print1122();
        }

        static void print1122()
        {
            string bitmapFilePath = @"C:\Users\Виктор\OneDrive\Рабочий стол\Текстовый документ (2).svg";
            int w, h;
            Bitmap b = new Bitmap(bitmapFilePath);
            w = b.Width; h = b.Height;
            byte[] bitmapFileData = System.IO.File.ReadAllBytes(bitmapFilePath);
            int fileSize = bitmapFileData.Length;

            // The following is known about test.bmp.  It is up to the developer
            // to determine this information for bitmaps besides the given test.bmp.
            int bitmapDataOffset = int.Parse(bitmapFileData[10].ToString()); ;
            int width = w; // int.Parse(bitmapFileData[18].ToString()); ;
            int height = h; // int.Parse(bitmapFileData[22].ToString()); ;
            int bitsPerPixel = int.Parse(bitmapFileData[28].ToString()); // Monochrome image required!
            int bitmapDataLength = bitmapFileData.Length - bitmapDataOffset;
            double widthInBytes = Math.Ceiling(width / 8.0);

            while (widthInBytes % 4 != 0)
            {
                widthInBytes++;
            }
            // Copy over the actual bitmap data from the bitmap file.
            // This represents the bitmap data without the header information.
            byte[] bitmap = new byte[bitmapDataLength];
            Buffer.BlockCopy(bitmapFileData, bitmapDataOffset, bitmap, 0, bitmapDataLength);

            string valu2e = ASCIIEncoding.ASCII.GetString(bitmap);

            //byte[] ReverseBitmap = new byte[bitmapDataLength];

            // Invert bitmap colors
            for (int i = 0; i < bitmapDataLength; i++)
            {
                bitmap[i] ^= 0xFF;
            }
            // Create ASCII ZPL string of hexadecimal bitmap data
            string ZPLImageDataString = BitConverter.ToString(bitmap);
            ZPLImageDataString = ZPLImageDataString.Replace("-", string.Empty);

            // Create ZPL command to print image
            string ZPLCommand = string.Empty;

            ZPLCommand += "^XA";
            ZPLCommand += "^PMY^FO20,20";
            ZPLCommand +=
            "^GFA, " +
            bitmapDataLength.ToString() + "," +
            bitmapDataLength.ToString() + "," +
            widthInBytes.ToString() + "," +
            ZPLImageDataString;

            ZPLCommand += "^XZ";

            System.IO.StreamWriter sr = new StreamWriter(@"D:\logoImage.zpl", false, System.Text.Encoding.Default);

            sr.Write(ZPLCommand);
            sr.Close();
        }
        private void PrintFile(string fileName)
        {
            using (var wb = new WebBrowser())
            {
                wb.Navigate(fileName);
                while (wb.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
                wb.ShowPrintDialog();//"Hewlett-Packard HP Color LaserJet CM1017"
            }
        }
    }

    public class SvgConvert
    {
        //преобразует SVG, заданный строкой svgcontent, в Bitmap заданных размеров и записывает его в поток output
        public static void ToBitmap(string svgcontent, int w_image, int h_image, System.IO.Stream output)
        {
            RECTL rcClient = new RECTL();
            
            bool b = SystemParametersInfo(SPI_GETWORKAREA, 0, ref rcClient, 0);
            if (b == false) { rcClient.bottom = 480; rcClient.right = 640; }

            int width = (int)(rcClient.right - rcClient.left);
            int height = (int)(rcClient.bottom - rcClient.top);

            IntPtr screendc = GetDC(IntPtr.Zero);

            string svghtml =
                "<html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=11\" /></head><body>" + svgcontent + "</body></html>";

            mshtml.HTMLDocument doc = null;
            mshtml.IHTMLDocument2 d2 = null;
            IOleObject pObj = null;
            IViewObject pView = null;

            try
            {
                doc = new mshtml.HTMLDocument(); //создание документа
                d2 = (mshtml.IHTMLDocument2)doc;
                int hr;

                //установка размера документа
                pObj = (IOleObject)d2;
                SIZEL sz = new SIZEL();

                sz.x = (uint)MulDiv(width, HIMETRIC_INCH, GetDeviceCaps(screendc, LOGPIXELSX));
                sz.y = (uint)MulDiv(height, HIMETRIC_INCH, GetDeviceCaps(screendc, LOGPIXELSY));

                hr = pObj.SetExtent((int)System.Runtime.InteropServices.ComTypes.DVASPECT.DVASPECT_CONTENT, ref sz);
                if (hr != 0) throw Marshal.GetExceptionForHR(hr);

                //запись SVG в документ
                d2.write(svghtml);
                d2.close();

                //преобразование в Bitmap
                pView = (IViewObject)d2;
                Bitmap bmp = new Bitmap(w_image, h_image);
                Graphics g = Graphics.FromImage(bmp);
                using (g)
                {

                    IntPtr hdc = g.GetHdc();
                    hr = pView.Draw((int)System.Runtime.InteropServices.ComTypes.DVASPECT.DVASPECT_CONTENT,
                        -1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, hdc, ref rcClient, IntPtr.Zero,
                        IntPtr.Zero, 0);
                    if (hr != 0) throw Marshal.GetExceptionForHR(hr);
                    g.ReleaseHdc(hdc);
                }

                //сохранение
                bmp.Save(output, System.Drawing.Imaging.ImageFormat.Bmp);


            }
            finally
            {
                //освобождение ресурсов                
                if (d2 != null) Marshal.ReleaseComObject(d2);
                if (pObj != null) Marshal.ReleaseComObject(pObj);
                if (pView != null) Marshal.ReleaseComObject(pView);
                if (doc != null) Marshal.ReleaseComObject(doc);
            }
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool SystemParametersInfo(int nAction, int nParam, ref RECTL rc, int nUpdate);

        public static int MulDiv(int number, int numerator, int denominator)
        {
            return (int)(((long)number * numerator) / denominator);
        }

        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        const int HIMETRIC_INCH = 2540;
        const int SPI_GETWORKAREA = 48;

    }

    [ComImport()]
    [GuidAttribute("0000010d-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IViewObject
    {
        int Draw([MarshalAs(UnmanagedType.U4)] int dwDrawAspect, int lindex, IntPtr pvAspect, IntPtr ptd, IntPtr hdcTargetDev, IntPtr hdcDraw,
            ref RECTL lprcBounds, IntPtr lprcWBounds, IntPtr pfnContinue, int dwContinue);
        int a();
        int b();
        int c();
        int d();
        int e();
    }

    [ComImport()]
    [Guid("00000112-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleObject
    {
        void f();
        void g();
        void SetHostNames(object szContainerApp, object szContainerObj);
        void Close(uint dwSaveOption);
        void SetMoniker(uint dwWhichMoniker, object pmk);
        void GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk);
        void x();
        void y();
        void DoVerb(uint iVerb, uint lpmsg, object pActiveSite, uint lindex, uint hwndParent, uint lprcPosRect);
        void EnumVerbs(ref object ppEnumOleVerb);
        void Update();
        void IsUpToDate();
        void GetUserClassID(uint pClsid);
        void GetUserType(uint dwFormOfType, uint pszUserType);
        int SetExtent(uint dwDrawAspect, ref SIZEL psizel);
        void GetExtent(uint dwDrawAspect, uint psizel);
        void Advise(object pAdvSink, uint pdwConnection);
        void Unadvise(uint dwConnection);
        void EnumAdvise(ref object ppenumAdvise);
        void GetMiscStatus(uint dwAspect, uint pdwStatus);
        void SetColorScheme(object pLogpal);
    };

    public struct RECTL
    {
        public uint left;
        public uint top;
        public uint right;
        public uint bottom;
    }

    public struct SIZEL
    {
        public uint x;
        public uint y;
    }
}
