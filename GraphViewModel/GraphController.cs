using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using SharpGraph.GraphModel;
using SharpGraph.GraphViewModel.Properties;

namespace SharpGraph.GraphViewModel {
    public class GraphController : INotifyPropertyChanged {
        private string _originalInputFile;
        private IGraph _originalGraph;
        private IGraph _originalLayoutGraph;
        private WpfGraph _originalWpfGraph;


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

        private void InitializeOriginalGraph(string filename) {
            _originalGraph = GraphParser.GraphParser.GetGraph(new FileInfo(filename));
            ReloadOriginalGraphLayout();
        }

        private void ReloadOriginalGraphLayout() {
            _originalLayoutGraph = GraphParser.GraphParser.GetGraphLayout(_originalGraph.ToDot());
            OriginalWpfGraph = new WpfGraph(_originalLayoutGraph);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
