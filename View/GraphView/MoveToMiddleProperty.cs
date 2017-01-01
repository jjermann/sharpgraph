using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SharpGraph.GraphView {
    public class CenterConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if ((values[0] == DependencyProperty.UnsetValue) || (values[1] == DependencyProperty.UnsetValue)) {
                return DependencyProperty.UnsetValue;
            }

            var width = (double) values[0];
            var height = (double) values[1];

            return new TranslateTransform(-width/2, -height/2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class Mover : DependencyObject {
        public static readonly DependencyProperty MoveToMiddleProperty =
            DependencyProperty.RegisterAttached("MoveToMiddle", typeof(bool), typeof(Mover),
                new PropertyMetadata(false, PropertyChangedCallback));

        public static void SetMoveToMiddle(UIElement element, bool value) {
            element.SetValue(MoveToMiddleProperty, value);
        }

        public static bool GetMoveToMiddle(UIElement element) {
            return (bool) element.GetValue(MoveToMiddleProperty);
        }

        private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var element = sender as FrameworkElement;
            if (element == null) {
                return;
            }

            if ((bool) e.NewValue) {
                var multiBinding = new MultiBinding {Converter = new CenterConverter()};
                multiBinding.Bindings.Add(new Binding("ActualWidth") {Source = element});
                multiBinding.Bindings.Add(new Binding("ActualHeight") {Source = element});
                element.SetBinding(UIElement.RenderTransformProperty, multiBinding);
            } else {
                element.ClearValue(UIElement.RenderTransformProperty);
            }
        }
    }
}