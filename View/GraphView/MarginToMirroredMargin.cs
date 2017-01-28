using System;
using System.Windows;
using System.Windows.Data;

namespace SharpGraph {
    [ValueConversion(typeof(string), typeof(Thickness))]
    public class MarginToMirroredMargin : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var margin = value as string;
            if (string.IsNullOrEmpty(margin)) {
                return DependencyProperty.UnsetValue;
            }
            var thicknessConverter = new ThicknessConverter();
            var convertFromString = thicknessConverter.ConvertFromInvariantString(margin);
            if (convertFromString == null) {
                return DependencyProperty.UnsetValue;
            }
            var thickness = (Thickness) convertFromString;
            var top = thickness.Top;
            var bottom = thickness.Bottom;
            thickness.Top = bottom;
            thickness.Bottom = top;
            return thickness;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}