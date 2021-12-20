using System;
using System.Runtime.InteropServices;

namespace SolidControls
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);
    }
}
