using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SharpGraph {
    [ValueConversion(typeof(ShapeData), typeof(Shape))]
    public class ShapeDataToShapeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var shapeStyle = parameter as Style;
            Shape shape;
            var shapeData = value as ShapeData;
            if (shapeData == null) {
                shape = new Ellipse();
            } else {
                switch (shapeData.Name) {
                    case "None":
                        shape = new Rectangle();
                        break;
                    case "Rectangle":
                        shape = new Rectangle();
                        break;
                    case "Polygon":
                        var sides = shapeData.Sides;
                        var baseAngle = 2*Math.PI/sides;
                        var initialAngle = Math.PI/2.0 - baseAngle/2.0 + shapeData.Angle;
                        var pointCollection = new PointCollection();
                        for (var i = 0; i < sides; i++) {
                            var x = Math.Cos(initialAngle + baseAngle*i) + 1;
                            var y = Math.Sin(initialAngle + baseAngle*i) + 1;
                            pointCollection.Add(new Point(x, y));
                        }
                        shape = new Polygon {
                            Points = pointCollection,
                            Stretch = Stretch.Fill
                        };
                        break;
                    default:
                        shape = new Ellipse();
                        break;
                }
            }
            if (shapeStyle != null) {
                shape.Style = shapeStyle;
            }
            return shape;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}