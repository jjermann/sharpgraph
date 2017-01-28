using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using SharpGraph;

namespace ExampleGraphView {
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class GraphController : INotifyPropertyChanged {
        #region Private

        private IGraph m_currentLayoutGraph;
        private IGraph CurrentLayoutGraph {
            get { return m_currentLayoutGraph; }
            set {
                m_currentLayoutGraph = value;
                OnPropertyChanged();
            }
        }

        private IGraph m_originalGraph;
        private IGraph OriginalGraph {
            get { return m_originalGraph; }
            set {
                m_originalGraph = value;
                OnPropertyChanged();
            }
        }

        private void UpdateCurrentContent() {
            var nodeSelector = RestrictVisibility
                ? OriginalGraph.GetNodeSelector(SelectedNodeIds, NodeVisitStopFunction, NodeVisitAcceptFunction)
                : null;
            CurrentDotContent = OriginalGraph.ToDot(new DotFormatOptions {NodeSelector = nodeSelector});
            if (UpdateCurrentImage) {
                CurrentImage = GraphParser.GetGraphImage(CurrentDotContent);
            }
            CurrentDotLayoutContent = GraphParser.GetGraphLayoutDot(CurrentDotContent);
            CurrentLayoutGraph = GraphParser.GetGraph(CurrentDotLayoutContent);

            if (CurrentWpfGraph != null) {
                CurrentWpfGraph.Changed -= CurrentWpfGraphChanged;
            }
            CurrentWpfGraph = new WpfGraph(CurrentLayoutGraph, SelectedNodeIds);
            CurrentWpfGraph.Changed += CurrentWpfGraphChanged;
        }

        private void CurrentWpfGraphChanged(object sender, EventArgs e) {
            SelectedNodeIds = new List<string>(
                CurrentWpfGraph.WpfNodes.Where(wn => wn.IsSelected).Select(wn => wn.Id));
            RaiseContentChanged();
        }

        private void InitializeOriginalGraph(string filename) {
            //TODO: Figure out a better start selection resp. parse it...
            DeselectAll();
            OriginalDotContent = File.ReadAllText(filename);
            UpdateOriginalGraphFromDotContent();

            // This also updates the current context
            RestrictVisibility = false;
        }

        private void RestrictSelection() {
            SelectedNodeIds = new List<string>(SelectedNodeIds.Intersect(OriginalGraph.GetNodes().Select(n => n.Id)));
        }

        private void SelectAll() {
            SelectedNodeIds = new List<string>(OriginalGraph.GetNodes().Select(n => n.Id));
        }

        private void DeselectAll() {
            SelectedNodeIds = new List<string>();
        }

        private IList<string> SelectedNodeIds { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void UpdateOriginalGraphFromDotContent() {
            try {
                var graph = GraphParser.GetGraph(OriginalDotContent);
                GraphParser.CheckSyntax(OriginalDotContent);
                OriginalGraph = graph;
                ParseFailureMessage = "";
            } catch (Exception e) {
                ParseFailureMessage = e.Message;
            }
            if (OriginalGraph == null) {
                OriginalGraph = Graph.CreateGraph();
            }
            if (string.IsNullOrEmpty(ParseFailureMessage)) {
                RestrictSelection();
                RaiseContentChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Private
        #region OtherPublic

        public GraphController() {
            ContentChanged += CurrentContentChanged;
        }

        //TODO: Make this a command, only update the graph once at the end
        public void SelectNodesById(IList<string> idList) {
            var nodesToSelect = CurrentWpfGraph.WpfNodes.Where(n => idList.Contains(n.Id));
            foreach (var wpfNode in nodesToSelect) {
                wpfNode.IsSelected = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler ContentChanged;

        public void CurrentContentChanged(object sender, EventArgs e) {
            UpdateCurrentContent();
        }

        private void RaiseContentChanged() {
            ContentChanged?.Invoke(this, new EventArgs());
        }

        public enum UpdateMode {
            ManualUpdate,
            ImmediateUpdate
        }

        #endregion OtherPublic
        #region PublicCommands

        private RelayCommand m_selectAllCommand;
        public ICommand SelectAllCommand {
            get {
                return m_selectAllCommand ?? (m_selectAllCommand = new RelayCommand(
                           param => {
                               SelectAll();
                               RaiseContentChanged();
                           }));
            }
        }

        private RelayCommand m_deselectAllCommand;
        public ICommand DeselectAllCommand {
            get {
                return m_deselectAllCommand ?? (m_deselectAllCommand = new RelayCommand(
                           param => {
                               DeselectAll();
                               RaiseContentChanged();
                           }));
            }
        }

        private RelayCommand m_originalDotToOriginalGraph;
        public ICommand OriginalDotToOriginalGraph {
            get {
                return m_originalDotToOriginalGraph ?? (m_originalDotToOriginalGraph = new RelayCommand(
                           param => UpdateOriginalGraphFromDotContent()));
            }
        }

        private RelayCommand m_generateDotFromGraphCommand;
        public ICommand GenerateDotFromGraphCommand {
            get {
                return m_generateDotFromGraphCommand ?? (m_generateDotFromGraphCommand = new RelayCommand(
                           param => {
                               var dotFormat = new DotFormatOptions {
                                   OrderByName = DotOrderByName,
                                   ShowRedundantNodes = DotShowRedundantNodes
                               };
                               OriginalDotContent = OriginalGraph.ToDot(dotFormat);
                           },
                           param => OriginalGraph != null
                       ));
            }
        }

        private RelayCommand m_openFileCommand;
        public ICommand OpenFileCommand {
            get {
                return m_openFileCommand ?? (m_openFileCommand = new RelayCommand(
                           param => { OriginalInputFile = (string) param; },
                           param => !string.IsNullOrEmpty((string) param)
                       ));
            }
        }

        public string SavedDotFile { get; private set; }
        private RelayCommand m_saveDotFileCommand;
        public ICommand SaveDotFileCommand {
            get {
                return m_saveDotFileCommand ?? (m_saveDotFileCommand = new RelayCommand(
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
        private RelayCommand m_saveOriginalDotFileCommand;
        public ICommand SaveOriginalDotFileCommand {
            get {
                return m_saveOriginalDotFileCommand ?? (m_saveOriginalDotFileCommand = new RelayCommand(
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
        private RelayCommand m_saveImageFileCommand;
        public ICommand SaveImageFileCommand {
            get {
                return m_saveImageFileCommand ?? (m_saveImageFileCommand = new RelayCommand(
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

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Func<INode, bool> NodeVisitStopFunction { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Func<INode, bool> NodeVisitAcceptFunction { get; set; }

        private bool m_restrictVisibility;
        public bool RestrictVisibility {
            get { return m_restrictVisibility; }
            set {
                m_restrictVisibility = value;
                OnPropertyChanged();
                RaiseContentChanged();
            }
        }

        private UpdateMode m_graphUpdateMode;
        public UpdateMode GraphUpdateMode {
            get { return m_graphUpdateMode; }
            set {
                m_graphUpdateMode = value;
                OnPropertyChanged();
                if (m_graphUpdateMode == UpdateMode.ImmediateUpdate) {
                    UpdateOriginalGraphFromDotContent();
                }
            }
        }

        private bool m_updateCurrentImage;
        public bool UpdateCurrentImage {
            get { return m_updateCurrentImage; }
            set {
                m_updateCurrentImage = value;
                if (m_updateCurrentImage) {
                    RaiseContentChanged();
                }
            }
        }

        private string m_parseFailureMessage;
        public string ParseFailureMessage {
            get { return m_parseFailureMessage; }
            private set {
                m_parseFailureMessage = value;
                OnPropertyChanged();
            }
        }

        private string m_originalInputFile;
        public string OriginalInputFile {
            get { return m_originalInputFile; }
            set {
                m_originalInputFile = value;
                InitializeOriginalGraph(m_originalInputFile);
                OnPropertyChanged();
            }
        }

        private WpfGraph m_currentWpfGraph;
        public WpfGraph CurrentWpfGraph {
            get { return m_currentWpfGraph; }
            private set {
                m_currentWpfGraph = value;
                OnPropertyChanged();
            }
        }

        private string m_originalDotContent;
        public string OriginalDotContent {
            get { return m_originalDotContent; }
            set {
                m_originalDotContent = value;
                OnPropertyChanged();
                if (GraphUpdateMode == UpdateMode.ImmediateUpdate) {
                    UpdateOriginalGraphFromDotContent();
                }
            }
        }

        private string m_currentDotContent;
        public string CurrentDotContent {
            get { return m_currentDotContent; }
            private set {
                m_currentDotContent = value;
                OnPropertyChanged();
            }
        }

        private string m_currentDotLayoutContent;
        public string CurrentDotLayoutContent {
            get { return m_currentDotLayoutContent; }
            private set {
                m_currentDotLayoutContent = value;
                OnPropertyChanged();
            }
        }

        private Image m_currentImage;
        public Image CurrentImage {
            get { return m_currentImage; }
            private set {
                m_currentImage = value;
                OnPropertyChanged();
            }
        }

        private bool m_dotOrderByName;
        public bool DotOrderByName {
            get { return m_dotOrderByName; }
            set {
                m_dotOrderByName = value;
                OnPropertyChanged();
            }
        }

        private bool m_dotShowRedundantNodes;
        public bool DotShowRedundantNodes {
            get { return m_dotShowRedundantNodes; }
            set {
                m_dotShowRedundantNodes = value;
                OnPropertyChanged();
            }
        }

        #endregion PublicProperties
    }
}