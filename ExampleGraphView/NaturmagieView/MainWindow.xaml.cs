namespace ExampleGraphView {
    public sealed partial class MainWindow {
        public MainWindow(object dataContext) {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}