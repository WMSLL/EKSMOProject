using System;
using System.Runtime.InteropServices;

namespace SVGtoZPL22
{
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
}
