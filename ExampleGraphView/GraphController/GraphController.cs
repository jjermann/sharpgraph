﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using SharpGraph.GraphModel;
using SharpGraph.GraphViewModel;

namespace ExampleGraphView {
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class GraphController : INotifyPropertyChanged {
        #region Private

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

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Func<INode, bool> NodeVisitStopFunction { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Func<INode, bool> NodeVisitAcceptFunction { get; set; }

        private void UpdateCurrentContent() {
            var nodeSelector = RestrictVisibility
                ? OriginalGraph.GetNodeSelector(SelectedNodeIds, NodeVisitStopFunction, NodeVisitAcceptFunction)
                : null;
            CurrentDotContent = OriginalGraph.ToDot(nodeSelector: nodeSelector);
            if (UpdateCurrentImage) {
                CurrentImage = SharpGraph.GraphParser.GraphParser.GetGraphImage(CurrentDotContent);
            }
            CurrentDotLayoutContent = SharpGraph.GraphParser.GraphParser.GetGraphLayoutDot(CurrentDotContent);
            CurrentLayoutGraph = SharpGraph.GraphParser.GraphParser.GetGraph(CurrentDotLayoutContent);

            if (CurrentWpfGraph != null) {
                CurrentWpfGraph.Changed -= CurrentWpfGraphChanged;
            }
            CurrentWpfGraph = new WpfGraph(CurrentLayoutGraph, SelectedNodeIds);
            CurrentWpfGraph.Changed += CurrentWpfGraphChanged;
        }

        private void CurrentWpfGraphChanged(object sender, EventArgs e) {
            SelectedNodeIds = new ObservableCollection<string>(
                CurrentWpfGraph.WpfNodes.Where(wn => wn.IsSelected).Select(wn => wn.Id));
            UpdateCurrentContent();
        }

        private void InitializeOriginalGraph(string filename) {
            OriginalGraph = SharpGraph.GraphParser.GraphParser.GetGraph(new FileInfo(filename));
            OriginalDotContent = OriginalGraph.ToDot();
            SharpGraph.GraphParser.GraphParser.CheckSyntax(OriginalDotContent);
            //TODO: Figure out a better start selection resp. parse it...
            DeselectAll();
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

        private void UpdateOriginalGraphFromDotContent() {
            try {
                var graph = SharpGraph.GraphParser.GraphParser.GetGraph(OriginalDotContent);
                SharpGraph.GraphParser.GraphParser.CheckSyntax(OriginalDotContent);
                OriginalGraph = graph;
                ParseFailureMessageOriginalDotContent = "";
            } catch (Exception e) {
                ParseFailureMessageOriginalDotContent = e.Message;
            }
            if (string.IsNullOrEmpty(ParseFailureMessageOriginalDotContent)) {
                RestrictSelection();
                UpdateCurrentContent();
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

        private RelayCommand _selectAllCommand;
        public ICommand SelectAllCommand {
            get {
                return _selectAllCommand ?? (_selectAllCommand = new RelayCommand(
                           param => {
                               SelectAll();
                               UpdateCurrentContent();
                           }));
            }
        }

        private RelayCommand _deselectAllCommand;
        public ICommand DeselectAllCommand {
            get {
                return _deselectAllCommand ?? (_deselectAllCommand = new RelayCommand(
                           param => {
                               DeselectAll();
                               UpdateCurrentContent();
                           }));
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

        private bool _restrictVisibility;
        public bool RestrictVisibility {
            get { return _restrictVisibility; }
            set {
                _restrictVisibility = value;
                OnPropertyChanged();
                UpdateCurrentContent();
            }
        }

        private UpdateMode _graphUpdateMode;
        public UpdateMode GraphUpdateMode {
            get { return _graphUpdateMode; }
            set {
                _graphUpdateMode = value;
                OnPropertyChanged();
            }
        }

        private bool _updateCurrentImage;
        public bool UpdateCurrentImage {
            get { return _updateCurrentImage; }
            set {
                _updateCurrentImage = value;
                if (_updateCurrentImage) {
                    UpdateCurrentContent();
                }
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

        private string _currentDotContent;
        public string CurrentDotContent {
            get { return _currentDotContent; }
            private set {
                _currentDotContent = value;
                OnPropertyChanged();
            }
        }

        private string _currentDotLayoutContent;
        public string CurrentDotLayoutContent {
            get { return _currentDotLayoutContent; }
            private set {
                _currentDotLayoutContent = value;
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

        public IList<string> SelectedNodeIds { get; set; }

        #endregion PublicProperties
    }
}