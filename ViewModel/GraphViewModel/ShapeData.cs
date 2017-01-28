using System.Diagnostics.CodeAnalysis;

namespace SharpGraph {
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ShapeData {
        public string Name { get; set; } = "ellipse";
        public int Sides { get; set; } = 4;
        public int Layers { get; set; } = 1;
        public bool HasDiagonals { get; set; }
        public double Angle { get; set; }
    }
}