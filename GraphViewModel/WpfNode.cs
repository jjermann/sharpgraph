using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfNode {
        private readonly INode _nodeBehind;

        public WpfNode(INode nodeBehind) {
            _nodeBehind = nodeBehind;
        }

        public string Label => _nodeBehind.HasAttribute("label") ? _nodeBehind.GetAttribute("label") : _nodeBehind.Id;
        public double Width => ConvertToDouble(_nodeBehind.GetAttribute("width").Trim('"'),60);
        public double Height => ConvertToDouble(_nodeBehind.GetAttribute("height").Trim('"'),60);
        public double X => ConvertToDouble(_nodeBehind.GetAttribute("pos").Trim('"').Split(',')[0]);
        public double Y => ConvertToDouble(_nodeBehind.GetAttribute("pos").Trim('"').Split(',')[1]);
        
        private double ConvertToDouble(string input, double factor=1) {
            var foo = double.Parse(input)*factor;
            return foo;
        }
    }
}
