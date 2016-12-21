using SharpGraph.GraphModel;

namespace SharpGraph.GraphViewModel {
    public class WpfSubGraph {
        private readonly ISubGraph _subGraphBehind;

        public WpfSubGraph(ISubGraph subGraphBehind) {
            _subGraphBehind = subGraphBehind;
        }

        public string Label => _subGraphBehind.HasAttribute("label") ? _subGraphBehind.GetAttribute("label") : _subGraphBehind.Id;
    }
}
