using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using SharpGraph;

namespace ExampleGraphView {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
         MessageId = "Naturmagie")]
    public partial class NaturmagieView {
        private GraphController NaturmagieViewModel { get; set; }
        private GraphController BeherrschungViewModel { get; set; }

        private void NaturmagieView_Startup(object sender, StartupEventArgs e) {
            const string beherrschungFile = "naturmagie_beherrschung_basic.dot";
            BeherrschungViewModel = new GraphController();
            BeherrschungViewModel.OpenFileCommand.Execute(beherrschungFile);
            BeherrschungViewModel.NodeVisitStopFunction = n => !string.IsNullOrEmpty(
                WpfHelper.IdToText(n.HasAttribute("label")
                    ? n.GetAttribute("label")
                    : n.Id));
            BeherrschungViewModel.NodeVisitAcceptFunction = n => true;
            BeherrschungViewModel.SelectNodesById(new List<string> {"\"Beherrschung der Naturmagie\""});
            BeherrschungViewModel.RestrictVisibility = true;

            const string naturmagieFile = "naturmagie_basic.dot";
            NaturmagieViewModel = new GraphController();
            NaturmagieViewModel.OpenFileCommand.Execute(naturmagieFile);
            NaturmagieViewModel.NodeVisitStopFunction = n => !string.IsNullOrEmpty(
                WpfHelper.IdToText(n.HasAttribute("label")
                    ? n.GetAttribute("label")
                    : n.Id));
            NaturmagieViewModel.NodeVisitAcceptFunction = BeherrschungAccept;
            NaturmagieViewModel.SelectNodesById(new List<string> {"Naturmagie"});
            NaturmagieViewModel.RestrictVisibility = true;

            BeherrschungViewModel.ContentChanged += NaturmagieViewModel.CurrentContentChanged;

            var beherrschungWindow = new MainWindow(BeherrschungViewModel) {Title = "Naturmagie - Beherrschung"};
            var naturmagieWindow = new MainWindow(NaturmagieViewModel) {Title = "Naturmagie - Fähigkeiten"};
            beherrschungWindow.Show();
            naturmagieWindow.Show();
        }

        private bool BeherrschungAccept(INode node) {
            var label = WpfHelper.IdToText(node.HasAttribute("label")
                ? node.GetAttribute("label")
                : node.Id);
            var regex = new Regex(@"^.*\nV: (?<prereq>.*)$", RegexOptions.Singleline);
            var prereqStr = regex.Match(label).Groups["prereq"].Value;
            if (string.IsNullOrEmpty(prereqStr)) {
                return true;
            }
            var prereqs = prereqStr.Split(',', '\n').Select(s => s.Trim()).ToList();
            var knowledge = new List<string> {
                "WI",
                "WII",
                "WIII",
                "Weltenzauber",
                "MI",
                "MII",
                "MIII",
                "BI",
                "BII",
                "Erschaffen",
                "Binden",
                "Beschwören"
            };
            var knowledgePrereqs = prereqs.Intersect(knowledge);
            var ids = BeherrschungViewModel.CurrentWpfGraph.WpfNodes.Where(n => n.IsSelected).Select(n => n.Id.Trim());
            return !knowledgePrereqs.Except(ids).Any();
        }
    }
}