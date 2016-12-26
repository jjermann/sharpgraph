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
        public GraphController() {
            VisibleNodeIds = new ObservableCollection<string>();
            VisibleNodeIds.CollectionChanged += (sender, e) => {
                if (!IsDirty) {
                    UpdateCurrentContent();
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #region Private

        private bool IsDirty { get; set; }

        private IGraph _currentLayoutGraph;
        private IGraph CurrentLayoutGraph {
            get { return _currentLayoutGraph; }
            set {
                _currentLayoutGraph = value;
                OnPropertyChanged();
            }
        }

        private IGraph _originalGraph;
        private IGraph OriginalGraph {
            get { return _originalGraph; }
            set {
                _originalGraph = value;
                OnPropertyChanged();
                UpdateOriginalContent();
            }
        }

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

        #endregion Private
        #region PublicProperties

        private RelayCommand _openFileCommand;
        public ICommand OpenFileCommand {
            get {
                return _openFileCommand ?? (_openFileCommand = new RelayCommand(
                           param => { OriginalInputFile = (string) param; },
                           param => !string.IsNullOrEmpty((string) param)
                       ));
            }
        }

        private bool _hasSavedDotFile;
        public bool HasSavedDotFile {
            get { return _hasSavedDotFile; }
            set {
                _hasSavedDotFile = value;
                OnPropertyChanged();
            }
        }
        private string _savedDotFile;
        private string SavedDotFile {
            get { return _savedDotFile; }
            set {
                _savedDotFile = value;
                HasSavedDotFile = !string.IsNullOrEmpty(_savedDotFile);
            }
        }
        private RelayCommand _saveDotFileCommand;
        public ICommand SaveDotFileCommand {
            get {
                return _saveDotFileCommand ?? (_saveDotFileCommand = new RelayCommand(
                           param => {
                               var filename = (string) param;
                               if (filename != null) {
                                   SavedDotFile = filename;
                               }
                               File.WriteAllText(SavedDotFile, CurrentDotContent);
                           }
                       ));
            }
        }

        private bool _hasSavedImageFile;
        public bool HasSavedImageFile {
            get { return _hasSavedImageFile; }
            set {
                _hasSavedImageFile = value;
                OnPropertyChanged();
            }
        }
        private string _savedImageFile;
        private string SavedImageFile {
            get { return _savedImageFile; }
            set {
                _savedImageFile = value;
                HasSavedImageFile = !string.IsNullOrEmpty(_savedImageFile);
            }
        }
        private RelayCommand _saveImageFileCommand;
        public ICommand SaveImageFileCommand {
            get {
                return _saveImageFileCommand ?? (_saveImageFileCommand = new RelayCommand(
                           param => {
                               var filename = (string) param;
                               if (filename != null) {
                                   SavedImageFile = filename;
                               }
                               CurrentImage.Save(SavedImageFile);
                           }
                       ));
            }
        }

        private string _originalInputFile;
        public string OriginalInputFile {
            get { return _originalInputFile; }
            set {
                _originalInputFile = value;
                InitializeOriginalGraph(_originalInputFile);
                OnPropertyChanged();
            }
        }

        private WpfGraph _currentWpfGraph;
        public WpfGraph CurrentWpfGraph {
            get { return _currentWpfGraph; }
            private set {
                _currentWpfGraph = value;
                OnPropertyChanged();
            }
        }

        private string _originalDotContent;
        public string OriginalDotContent {
            get { return _originalDotContent; }
            private set {
                _originalDotContent = value;
                OnPropertyChanged();
            }
        }

        private Image _originalImage;
        public Image OriginalImage {
            get { return _originalImage; }
            private set {
                _originalImage = value;
                OnPropertyChanged();
            }
        }

        private string _currentDotContent;
        public string CurrentDotContent {
            get { return _currentDotContent; }
            private set {
                _currentDotContent = value;
                OnPropertyChanged();
            }
        }

        private Image _currentImage;
        public Image CurrentImage {
            get { return _currentImage; }
            private set {
                _currentImage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> VisibleNodeIds { get; }

        #endregion PublicProperties
    }
}