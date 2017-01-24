using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SharpGraph {
    [ValueConversion(typeof(List<double>), typeof(DoubleCollection))]
    public class ListToDoubleCollectionConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) {
            var listValue = value as List<double>;
            if (listValue == null) {
                return DependencyProperty.UnsetValue;
            }
            return new DoubleCollection(listValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}