using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using SharpGraph.GraphModel;
using SharpGraph.GraphViewModel.Properties;

namespace SharpGraph.GraphViewModel {
    public sealed class GraphController : INotifyPropertyChanged {
        private string _originalInputFile;
        private IGraph _originalGraph;
        private IGraph _originalLayoutGraph;
        private WpfGraph _originalWpfGraph;
        private Func<INode, bool> GetNodeSelector(IEnumerable<string> visibleIds) {
            if (visibleIds == null) {
                return null;
            }
            return node => visibleIds.Contains(node.Id);
        }

        public GraphController() {
            VisibleNodeIds = new ObservableCollection<string>();
        }

        public string OriginalInputFile {
            get { return _originalInputFile; }
            set {
                _originalInputFile = value;
                InitializeOriginalGraph(_originalInputFile);
                OnPropertyChanged();
            }
        }

        public WpfGraph OriginalWpfGraph {
            get { return _originalWpfGraph; }
            private set {
                _originalWpfGraph = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> VisibleNodeIds { get; }

        private void InitializeOriginalGraph(string filename) {
            _originalGraph = GraphParser.GraphParser.GetGraph(new FileInfo(filename));
            foreach (var node in _originalGraph.GetNodes()) {
                VisibleNodeIds.Add(node.Id);
            }
            ReloadOriginalGraphLayout();
        }

        private void ReloadOriginalGraphLayout() {
            var graphDot = _originalGraph.ToDot(nodeSelector: GetNodeSelector(VisibleNodeIds));
            _originalLayoutGraph = GraphParser.GraphParser.GetGraphLayout(graphDot);
            OriginalWpfGraph = new WpfGraph(_originalLayoutGraph);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
