using System.Linq;
using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace ExampleConsoleProgram {
    public static class DiffHelper {
        private const int Margin = 2;

        public static string GetDiff(string a, string b) {
            var sb = new StringBuilder();
            var d = new Differ();
            var builder = new InlineDiffBuilder(d);
            var result = builder.BuildDiffModel(a, b);

            if (result.Lines.FirstOrDefault(line => line.Type != ChangeType.Unchanged) == null) {
                return "<No difference>" + "\n\n";
            }
            var wasUnchanged = 0;
            foreach (var line in result.Lines) {
                var changed = line.Type != ChangeType.Unchanged;

                if (changed || (wasUnchanged < Margin)) {
                    if (line.Type == ChangeType.Inserted) {
                        sb.Append("+ ");
                    } else if (line.Type == ChangeType.Deleted) {
                        sb.Append("- ");
                    } else if (line.Type == ChangeType.Modified) {
                        sb.Append("* ");
                    } else if (line.Type == ChangeType.Imaginary) {
                        sb.Append("? ");
                    } else if (line.Type == ChangeType.Unchanged) {
                        sb.Append("  ");
                    }
                    sb.Append(line.Text + "\n");
                } else if (wasUnchanged == Margin) {
                    sb.Append("..." + "\n");
                }
                if (changed) {
                    wasUnchanged = 0;
                } else {
                    wasUnchanged++;
                }
            }

            return sb.ToString();
        }
    }
}