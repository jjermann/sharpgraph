﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ExampleGraphView {
    [ValueConversion(typeof(Image), typeof(BitmapSource))]
    public class ImageToBitmapSourceConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) {
            var myImage = (Image) value;
            Debug.Assert(myImage != null);
            using (var bitmap = new Bitmap(myImage)) {
                var bmpPt = bitmap.GetHbitmap();
                var bitmapSource =
                    System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bmpPt,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                //freeze bitmapSource and clear memory to avoid memory leaks
                bitmapSource.Freeze();
                NativeMethods.DeleteObject(bmpPt);

                return bitmapSource;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}