namespace ExampleGraphView {
    public sealed partial class MainDatabaseWindow {
        public MainDatabaseWindow(object dataContext) {
            InitializeComponent();
            DataContext = dataContext;
        }
    }
}