using System;
using System.Windows;
using System.Windows.Data;

namespace SharpGraph {
    public class Mover : DependencyObject {
        public static readonly DependencyProperty MoveToMiddleProperty =
            DependencyProperty.RegisterAttached("MoveToMiddle", typeof(bool), typeof(Mover),
                new PropertyMetadata(false, PropertyChangedCallback));

        public static void SetMoveToMiddle(UIElement element, bool value) {
            if (element == null) throw new ArgumentNullException(nameof(element));
            element.SetValue(MoveToMiddleProperty, value);
        }

        public static bool GetMoveToMiddle(UIElement element) {
            if (element == null) throw new ArgumentNullException(nameof(element));
            // ReSharper disable once PossibleNullReferenceException
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