using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
//Reference: System.Drawing
//Reference: Microsoft HTML Object Library
namespace SVGtoZPL22
{
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
}
