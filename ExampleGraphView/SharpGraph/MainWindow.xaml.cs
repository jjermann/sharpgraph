using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using Microsoft.Win32;
using SharpGraph;

namespace ExampleGraphView {
    public sealed partial class MainWindow : INotifyPropertyChanged {
        public MainWindow(string initialFile = null) {
            InitializeComponent();
            if (string.IsNullOrEmpty(initialFile)) {
                initialFile = "example.gv";
            }
            var vm = (GraphController) DataContext;
            if (vm.OpenFileCommand.CanExecute(initialFile)) {
                vm.OpenFileCommand.Execute(initialFile);
            }
            ShowDotOutput = false;
            ShowDotLayoutOutput = false;
            ShowImageOutput = false;
            ShowConnectedComponentsOutput = false;
            ShowDotInput = true;
        }

        private DotOutput DotOutputWindow { get; set; }
        private DotLayoutOutput DotLayoutOutputWindow { get; set; }
        private ImageOutput ImageOutputWindow { get; set; }
        private DotInput DotInputWindow { get; set; }
        private ConnectedComponentsOutput ConnectedComponentsOutputWindow { get; set; }

        private bool m_showDotOutput;
        public bool ShowDotOutput {
            get { return m_showDotOutput; }
            set {
                m_showDotOutput = value;
                if (m_showDotOutput) {
                    if (DotOutputWindow == null) {
                        DotOutputWindow = new DotOutput(DataContext);
                        DotOutputWindow.Show();
                    }
                } else {
                    if (DotOutputWindow != null) {
                        DotOutputWindow.Close();
                        DotOutputWindow = null;
                    }
                }
                OnPropertyChanged();
            }
        }

        private bool m_showDotLayoutOutput;
        public bool ShowDotLayoutOutput {
            get { return m_showDotLayoutOutput; }
            set {
                m_showDotLayoutOutput = value;
                if (m_showDotLayoutOutput) {
                    if (DotLayoutOutputWindow == null) {
                        DotLayoutOutputWindow = new DotLayoutOutput(DataContext);
                        DotLayoutOutputWindow.Show();
                    }
                } else {
                    if (DotLayoutOutputWindow != null) {
                        DotLayoutOutputWindow.Close();
                        DotLayoutOutputWindow = null;
                    }
                }
                OnPropertyChanged();
            }
        }

        private bool m_showImageOutput;
        public bool ShowImageOutput {
            get { return m_showImageOutput; }
            set {
                m_showImageOutput = value;
                var vm = (GraphController) DataContext;
                if (m_showImageOutput) {
                    vm.UpdateCurrentImage = true;
                    if (ImageOutputWindow == null) {
                        ImageOutputWindow = new ImageOutput(DataContext);
                        ImageOutputWindow.Show();
                    }
                } else {
                    vm.UpdateCurrentImage = false;
                    if (ImageOutputWindow != null) {
                        ImageOutputWindow.Close();
                        ImageOutputWindow = null;
                    }
                }
                OnPropertyChanged();
            }
        }

        private bool m_showConnectedComponentsOutput;
        public bool ShowConnectedComponentsOutput
        {
            get { return m_showConnectedComponentsOutput; }
            set
            {
                m_showConnectedComponentsOutput = value;
                var vm = (GraphController)DataContext;
                vm.IsConnectedComponentsEnabled = m_showConnectedComponentsOutput;
                if (m_showConnectedComponentsOutput)
                {
                    if (ConnectedComponentsOutputWindow == null)
                    {
                        ConnectedComponentsOutputWindow = new ConnectedComponentsOutput(DataContext);
                        ConnectedComponentsOutputWindow.Show();
                    }
                }
                else
                {
                    if (ConnectedComponentsOutputWindow != null)
                    {
                        ConnectedComponentsOutputWindow.Close();
                        ConnectedComponentsOutputWindow = null;
                    }
                }
                OnPropertyChanged();
            }
        }

        private bool m_showDotInput;
        public bool ShowDotInput {
            get { return m_showDotInput; }
            set {
                m_showDotInput = value;
                if (m_showDotInput) {
                    if (DotInputWindow == null) {
                        DotInputWindow = new DotInput(DataContext);
                        DotInputWindow.Show();
                    }
                } else {
                    if (DotInputWindow != null) {
                        DotInputWindow.Close();
                        DotInputWindow = null;
                    }
                }
                OnPropertyChanged();
            }
        }

        private RelayCommand m_openCommand;
        public ICommand OpenCommand {
            get {
                return m_openCommand ?? (m_openCommand = new RelayCommand(
                           param => {
                               var vm = (GraphController) DataContext;
                               var filename = FileDialogHandler<OpenFileDialog>.OpenDialog(vm.OriginalInputFile);
                               if ((filename != null) && vm.OpenFileCommand.CanExecute(filename)) {
                                   vm.OpenFileCommand.Execute(filename);
                               }
                           }
                       ));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}