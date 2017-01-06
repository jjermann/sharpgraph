﻿using System;
using System.Runtime.InteropServices;

namespace ExampleGraphView {
    public static class NativeMethods {
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);
    }
}