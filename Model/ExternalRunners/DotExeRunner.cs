using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpGraph {
    public class DotExeRunner<T> : IDotRunner<T> {
        private const string DefaultDotExecutablePath = @"C:\Program Files (x86)\Graphviz2.38\bin";

        public DotExeRunner(string argument, Func<StreamReader, T> outputProcessor) {
            DotExecutablePath = ConfigurationManager.AppSettings["GraphvizPath"] ?? DefaultDotExecutablePath;
            DotExecutable = "dot.exe";
            DotGraphLayoutArgument = argument;
            OutputProcessor = outputProcessor;
        }

        private string DotExecutablePath { get; }
        private string DotExecutable { get; }
        private string DotGraphLayoutArgument { get; }
        private Func<StreamReader, T> OutputProcessor { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability",
             "CA2000:Dispose objects before losing scope")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public T GetOutput(string input) {
            var output = default(T);
            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Arguments = DotGraphLayoutArgument,
                    FileName = Path.Combine(DotExecutablePath, DotExecutable),
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                }
            };
            process.Start();
            process.StandardInput.AutoFlush = true;
            process.StandardInput.Write(input);
            process.StandardInput.Close();

            string outputExceptionMessage = null;
            try {
                output = OutputProcessor(process.StandardOutput);
            } catch (Exception e) {
                outputExceptionMessage = e.Message;
            }

            var errorOutput = process.StandardError.ReadToEnd();
            process.WaitForExit();
            var exitCode = process.ExitCode;
            process.Dispose();

            if ((exitCode > 0) || (outputExceptionMessage != null)) {
                string exceptionMessage;
                if (exitCode > 0) {
                    exceptionMessage = errorOutput.Replace("Error: <stdin>: ", "") + "\n";
                    exceptionMessage += GetImprovedExceptionInformation(errorOutput, input);
                } else {
                    exceptionMessage = outputExceptionMessage + "\n";
                }
                throw new Exception(exceptionMessage);
            }

            return output;
        }

        private static string GetImprovedExceptionInformation(string msg, string source, int margin = 3) {
            var lineMatcher = new Regex(".* in line (?<line>[1-9][0-9]*) .*");
            var lineStr = lineMatcher.Match(msg).Groups["line"]?.Value;
            if (!string.IsNullOrEmpty(lineStr)) {
                var line = int.Parse(lineStr, CultureInfo.InvariantCulture);
                return GetLineContent(source, line, margin) + "\n";
            }
            return "";
        }

        private static string GetLineContent(string input, int line, int margin = 3) {
            return input.Replace("\r", "")
                .Split('\n')
                .Select((v, i) => new {v, i})
                .Where(e => Math.Abs(e.i - (line - 1)) <= margin)
                .Select(e => (e.i == line - 1 ? "-> " : "   ") + e.v)
                .Aggregate((v1, v2) => v1 + "\n" + v2);
        }
    }
}