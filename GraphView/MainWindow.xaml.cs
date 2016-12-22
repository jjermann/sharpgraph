using System.Windows;
namespace SharpGraph.GraphView {
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            ((GraphViewModel.GraphController) DataContext).InputFile = "example.dot";
        }
    }
}
