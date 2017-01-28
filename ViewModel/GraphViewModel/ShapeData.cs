namespace SharpGraph {
    public class ShapeData {
        public string Name { get; set; } = "ellipse";
        public int Sides { get; set; } = 4;
        public int Layers { get; set; } = 1;
        public bool HasDiagonals { get; set; } = false;
        public double Angle { get; set; } = 0;
    }
}