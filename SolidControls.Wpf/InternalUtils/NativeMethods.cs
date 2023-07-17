using System;
using System.Runtime.InteropServices;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);
    }
}
