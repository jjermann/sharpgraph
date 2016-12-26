using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SharpGraph.GraphModel;
using SharpGraph.GraphViewModel;
using SharpGraph.GraphViewModel.Properties;

namespace SharpGraph.GraphControllerViewModel {
    public sealed class GraphController : INotifyPropertyChanged {
        private IGraph _currentLayoutGraph;
        private WpfGraph _currentWpfGraph;
        private RelayCommand _openFileCommand;
        private IGraph _originalGraph;
        private string _originalInputFile;
        private string _originalDotContent;
        private Image _originalImage;
        private string _currentDotContent;
        private Image _currentImage;

        public GraphController() {
            VisibleNodeIds = new ObservableCollection<string>();
            VisibleNodeIds.CollectionChanged += (sender, e) => {
                if (!IsDirty) {
                    UpdateCurrentContent();
                }
            };
        }

        private bool IsDirty { get; set; }

        private IGraph CurrentLayoutGraph {
            get { return _currentLayoutGraph; }
            set {
                _currentLayoutGraph = value;
                OnPropertyChanged();
            }
        }

        private IGraph OriginalGraph {
            get { return _originalGraph; }
            set {
                _originalGraph = value;
                OnPropertyChanged();
                UpdateOriginalContent();
            }
        }

        public ICommand OpenFileCommand {
            get {
                return _openFileCommand ?? (_openFileCommand = new RelayCommand(
                           param => { OriginalInputFile = (string) param; },
                           param => !string.IsNullOrEmpty((string) param
                           )));
            }
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

        public string OriginalDotContent {
            get { return _originalDotContent; }
            private set {
                _originalDotContent = value; 
                OnPropertyChanged();
            }
        }

        public Image OriginalImage {
            get { return _originalImage; }
            private set {
                _originalImage = value; 
                OnPropertyChanged();
            }
        }

        public string CurrentDotContent {
            get { return _currentDotContent; }
            private set {
                _currentDotContent = value;
                OnPropertyChanged();
            }
        }

        public Image CurrentImage {
            get { return _currentImage; }
            private set {
                _currentImage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> VisibleNodeIds { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateCurrentContent() {
            CurrentDotContent = OriginalGraph.ToDot(nodeSelector: GetNodeSelector(VisibleNodeIds));
            CurrentImage = GraphParser.GraphParser.GetGraphImage(CurrentDotContent);
            CurrentLayoutGraph = GraphParser.GraphParser.GetGraphLayout(CurrentDotContent);
            CurrentWpfGraph = new WpfGraph(CurrentLayoutGraph);
        }

        private void UpdateOriginalContent() {
            OriginalDotContent = OriginalGraph.ToDot();
            OriginalImage = GraphParser.GraphParser.GetGraphImage(OriginalDotContent);
        }

        private Func<INode, bool> GetNodeSelector(IEnumerable<string> visibleIds) {
            if (visibleIds == null) {
                return null;
            }
            return node => visibleIds.Contains(node.Id);
        }

        private void InitializeOriginalGraph(string filename) {
            OriginalGraph = GraphParser.GraphParser.GetGraph(new FileInfo(filename));

            IsDirty = true;
            foreach (var node in OriginalGraph.GetNodes()) {
                VisibleNodeIds.Add(node.Id);
            }
            IsDirty = false;
            UpdateCurrentContent();
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}