using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SharpGraph {
    public class CenterConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if ((values[0] == DependencyProperty.UnsetValue) || (values[1] == DependencyProperty.UnsetValue)) {
                return DependencyProperty.UnsetValue;
            }

            var width = (double) values[0];
            var height = (double) values[1];

            return new TranslateTransform(-width/2, height/2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}