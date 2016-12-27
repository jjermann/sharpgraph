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
                if (!IsDirty && RestrictVisibility) {
                    UpdateCurrentContent();
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #region Private

        private bool IsDirty { get; set; }
        private bool RestrictVisibility {
            get { return _restrictVisibility; }
            set {
                _restrictVisibility = value;
                UpdateCurrentContent();
            }
        }

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
            }
        }

        private void UpdateCurrentContent() {
            var nodeSelector = RestrictVisibility
                ? GetNodeSelector(VisibleNodeIds)
                : null;
            CurrentDotContent = OriginalGraph.ToDot(nodeSelector: nodeSelector);
            CurrentImage = GraphParser.GraphParser.GetGraphImage(CurrentDotContent);
            CurrentLayoutGraph = GraphParser.GraphParser.GetGraphLayout(CurrentDotContent);
            CurrentWpfGraph = new WpfGraph(CurrentLayoutGraph);
        }

        private Func<INode, bool> GetNodeSelector(IEnumerable<string> visibleIds) {
            if (visibleIds == null) {
                return null;
            }
            return node => visibleIds.Contains(node.Id);
        }

        private void InitializeOriginalGraph(string filename) {
            OriginalGraph = GraphParser.GraphParser.GetGraph(new FileInfo(filename));
            OriginalDotContent = OriginalGraph.ToDot();

            IsDirty = true;
            foreach (var node in OriginalGraph.GetNodes()) {
                VisibleNodeIds.Add(node.Id);
            }
            IsDirty = false;
            //This will also update the current content...
            RestrictVisibility = true;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Private
        #region PublicProperties

        private RelayCommand _originalDotToOriginalGraph;
        public ICommand OriginalDotToOriginalGraph {
            get {
                return _originalDotToOriginalGraph ?? (_originalDotToOriginalGraph = new RelayCommand(
                           param => {
                               OriginalGraph = GraphParser.GraphParser.GetGraph(OriginalDotContent);
                               //This will also update the current content
                               RestrictVisibility = false;
                           }
                       ));
            }
        }

        private RelayCommand _openFileCommand;
        public ICommand OpenFileCommand {
            get {
                return _openFileCommand ?? (_openFileCommand = new RelayCommand(
                           param => { OriginalInputFile = (string) param; },
                           param => !string.IsNullOrEmpty((string) param)
                       ));
            }
        }

        private string _savedDotFile;
        public string SavedDotFile {
            get { return _savedDotFile; }
            private set {
                _savedDotFile = value;
                OnPropertyChanged();
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

        private string _savedOriginalDotFile;
        public string SavedOriginalDotFile {
            get { return _savedOriginalDotFile; }
            private set {
                _savedOriginalDotFile = value;
                OnPropertyChanged();
            }
        }
        private RelayCommand _saveOriginalDotFileCommand;
        public ICommand SaveOriginalDotFileCommand {
            get {
                return _saveOriginalDotFileCommand ?? (_saveOriginalDotFileCommand = new RelayCommand(
                           param => {
                               var filename = (string) param;
                               if (filename != null) {
                                   SavedOriginalDotFile = filename;
                               }
                               File.WriteAllText(SavedOriginalDotFile, OriginalDotContent);
                           }
                       ));
            }
        }

        private string _savedImageFile;
        public string SavedImageFile {
            get { return _savedImageFile; }
            private set {
                _savedImageFile = value;
                OnPropertyChanged();
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
            set {
                _originalDotContent = value;
                OnPropertyChanged();
                OriginalImage = GraphParser.GraphParser.GetGraphImage(OriginalDotContent);
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
        private bool _restrictVisibility;
        public Image CurrentImage {
            get { return _currentImage; }
            private set {
                _currentImage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> VisibleNodeIds { get; private set; }

        #endregion PublicProperties
    }
}