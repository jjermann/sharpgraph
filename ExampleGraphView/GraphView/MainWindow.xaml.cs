namespace SharpGraph.GraphView {
    public partial class MainWindow {
        public MainWindow(string initialFile = null) {
            InitializeComponent();
            if (string.IsNullOrEmpty(initialFile)) {
                initialFile = "example.dot";
            }
            ((GraphControllerViewModel.GraphController) DataContext).OriginalInputFile = initialFile;

            DotOutputWindow = new DotOutput(DataContext);
            ImageOutputWindow = new ImageOutput(DataContext);

            DotOutputWindow.Show();
            ImageOutputWindow.Show();
        }

        private DotOutput DotOutputWindow { get; }
        private ImageOutput ImageOutputWindow { get; }
    }
}