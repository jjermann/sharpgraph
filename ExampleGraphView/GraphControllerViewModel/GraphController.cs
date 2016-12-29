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
        #region Private

        private bool IsDirty { get; set; }

        private bool _restrictVisibility;
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
                ? GetNeighbourNodeSelector(VisibleNodeIds)
                : null;
            CurrentDotContent = OriginalGraph.ToDot(nodeSelector: nodeSelector);
            CurrentImage = GraphParser.GraphParser.GetGraphImage(CurrentDotContent);
            CurrentLayoutGraph = GraphParser.GraphParser.GetGraphLayout(CurrentDotContent);
            CurrentWpfGraph = new WpfGraph(CurrentLayoutGraph);
        }

        //TODO: improve performance?
        private Func<INode, bool> GetNeighbourNodeSelector(IEnumerable<string> visibleIds) {
            if (visibleIds == null) {
                return null;
            }
            var visibleIdList = visibleIds.ToList();
            return node => {
                if (visibleIdList.Contains(node.Id)) {
                    return true;
                }
                var neighbours = node.Root.IsDirected
                    ? node.IncomingNeighbours()
                    : node.ConnectedNeighbours();
                return visibleIdList.Intersect(neighbours.Select(n => n.Id)).Any();
            };
        }

        // ReSharper disable once UnusedMember.Local
        private Func<INode, bool> GetVisibleNodeSelector(IEnumerable<string> visibleIds) {
            if (visibleIds == null) {
                return null;
            }
            return node => visibleIds.Contains(node.Id);
        }

        private void InitializeOriginalGraph(string filename) {
            OriginalGraph = GraphParser.GraphParser.GetGraph(new FileInfo(filename));
            OriginalDotContent = OriginalGraph.ToDot();
            OriginalImage = GraphParser.GraphParser.GetGraphImage(OriginalDotContent);

            IsDirty = true;
            foreach (var node in OriginalGraph.GetNodes()) {
                VisibleNodeIds.Add(node.Id);
            }
            IsDirty = false;
            //This will also update the current content...
            RestrictVisibility = true;
        }

        private void UpdateOriginalGraphFromDotContent() {
            try {
                var graph = GraphParser.GraphParser.GetGraph(OriginalDotContent);
                var originalImage = GraphParser.GraphParser.GetGraphImage(OriginalDotContent);
                OriginalGraph = graph;
                OriginalImage = originalImage;
                ParseFailureMessageOriginalDotContent = "";
            } catch (Exception e) {
                ParseFailureMessageOriginalDotContent = e.Message;
            }
            if (string.IsNullOrEmpty(ParseFailureMessageOriginalDotContent)) {
                //This will also update the current content
                RestrictVisibility = false;
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Private
        #region OtherPublic

        public event PropertyChangedEventHandler PropertyChanged;

        public enum UpdateMode {
            ManualUpdate,
            ImmediateUpdate
        }

        #endregion OtherPublic
        #region PublicCommands

        private RelayCommand _toggleNodeSelectionCommand;
        public ICommand ToggleNodeVisibilityCommand {
            get {
                return _toggleNodeSelectionCommand ?? (_toggleNodeSelectionCommand = new RelayCommand(
                           param => {
                               var id = (string) param;
                               if (VisibleNodeIds.Contains(id)) {
                                   VisibleNodeIds.Remove(id);
                               } else {
                                   VisibleNodeIds.Add(id);
                               }
                           },
                           param => RestrictVisibility
                       ));
            }
        }

        private RelayCommand _originalDotToOriginalGraph;
        public ICommand OriginalDotToOriginalGraph {
            get {
                return _originalDotToOriginalGraph ?? (_originalDotToOriginalGraph = new RelayCommand(
                           param => UpdateOriginalGraphFromDotContent()));
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

        public string SavedDotFile { get; private set; }
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

        public string SavedOriginalDotFile { get; private set; }
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

        public string SavedImageFile { get; private set; }
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

        #endregion PublicCommands
        #region PublicProperties

        private UpdateMode _graphUpdateMode;
        public UpdateMode GraphUpdateMode {
            get { return _graphUpdateMode; }
            set {
                _graphUpdateMode = value;
                OnPropertyChanged();
            }
        }

        private string _parseFailureMessageOriginalDotContent;
        public string ParseFailureMessageOriginalDotContent {
            get { return _parseFailureMessageOriginalDotContent; }
            private set {
                _parseFailureMessageOriginalDotContent = value;
                OnPropertyChanged();
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
                if (GraphUpdateMode == UpdateMode.ImmediateUpdate) {
                    UpdateOriginalGraphFromDotContent();
                }
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