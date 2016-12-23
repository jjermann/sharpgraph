namespace SharpGraph.GraphView {
    public partial class MainWindow {
        public MainWindow(string initialFile = null) {
            InitializeComponent();
            if (string.IsNullOrEmpty(initialFile)) {
                initialFile = "example.dot";
            }
            ((GraphViewModel.GraphController) DataContext).OriginalInputFile = initialFile;
        }
    }
}
