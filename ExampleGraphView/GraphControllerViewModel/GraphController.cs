using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using SharpGraph.GraphModel;
using SharpGraph.GraphViewModel;
using SharpGraph.GraphViewModel.Properties;

namespace SharpGraph.GraphControllerViewModel {
    public sealed class GraphController : INotifyPropertyChanged {
        private string _originalInputFile;
        private IGraph _originalGraph;
        private IGraph _currentLayoutGraph;
        private WpfGraph _currentWpfGraph;
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

        public WpfGraph CurrentWpfGraph {
            get { return _currentWpfGraph; }
            private set {
                _currentWpfGraph = value;
                OnPropertyChanged();
            }
        }

        public string OriginalDotContent => _originalGraph.ToDot();
        public Image OriginalImage => GraphParser.GraphParser.GetGraphImage(OriginalDotContent);
        public string CurrentDotContent => _originalGraph.ToDot(nodeSelector: GetNodeSelector(VisibleNodeIds));
        public Image CurrentImage => GraphParser.GraphParser.GetGraphImage(CurrentDotContent);
        public ObservableCollection<string> VisibleNodeIds { get; }

        private void InitializeOriginalGraph(string filename) {
            _originalGraph = GraphParser.GraphParser.GetGraph(new FileInfo(filename));
            foreach (var node in _originalGraph.GetNodes()) {
                VisibleNodeIds.Add(node.Id);
            }
            ReloadCurrentGraphLayout();
        }

        private void ReloadCurrentGraphLayout() {
            _currentLayoutGraph = GraphParser.GraphParser.GetGraphLayout(CurrentDotContent);
            CurrentWpfGraph = new WpfGraph(_currentLayoutGraph);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
