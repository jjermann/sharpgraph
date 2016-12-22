using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using SharpGraph.GraphModel;
using SharpGraph.GraphViewModel.Properties;

namespace SharpGraph.GraphViewModel {
    public class GraphController : INotifyPropertyChanged {
        private string _inputFile;
        private IGraph _originalGraph;
        private IGraph _originalGraphLayout;

        public IEnumerable<WpfNode> WpfNodes { get; private set; }
        public IEnumerable<WpfEdge> WpfEdges { get; private set; }
        public IEnumerable<WpfSubGraph> WpfSubGraphs { get; private set; }

        public string InputFile {
            get { return _inputFile; }
            set { InitializeGraph(value); }
        }

        private void InitializeGraph(string filename) {
            _inputFile = filename;
            _originalGraph = GraphParser.GraphParser.GetGraph(new FileInfo(_inputFile));
            ReloadGraphLayout();
        }

        private void ReloadGraphLayout() {
            _originalGraphLayout = GraphParser.GraphParser.GetGraphLayout(_originalGraph.ToDot());
            WpfNodes = _originalGraphLayout.GetNodes().Select(n => new WpfNode(n));
            WpfEdges = _originalGraphLayout.GetEdges().Select(e => new WpfEdge(e));
            WpfSubGraphs = _originalGraphLayout.GetSubGraphs().Select(g => new WpfSubGraph(g));
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged("");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
