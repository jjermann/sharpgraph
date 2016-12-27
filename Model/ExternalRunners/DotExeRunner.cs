using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpGraph.ExternalRunners {
    public class DotExeRunner<T> : IDotRunner<T> {
        public DotExeRunner(string argument, Func<StreamReader, T> outputProcessor) {
            DotExecutablePath = @"C:\Program Files (x86)\Graphviz2.38\bin";
            DotExecutable = "dot.exe";
            DotGraphLayoutArgument = argument;
            OutputProcessor = outputProcessor;
        }

        private string DotExecutablePath { get; }
        private string DotExecutable { get; }
        private string DotGraphLayoutArgument { get; }
        private Func<StreamReader, T> OutputProcessor { get; }

        public T GetOutput(string input) {
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

            var output = default(T);
            string outputExceptionMessage = null;
            try {
                output = OutputProcessor(process.StandardOutput);
            } catch (Exception e) {
                outputExceptionMessage = e.Message;
            }

            var errorOutput = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if ((process.ExitCode > 0) || (outputExceptionMessage != null)) {
                string exceptionMessage;
                if (process.ExitCode > 0) {
                    exceptionMessage = errorOutput.Replace("Error: <stdin>: ", "") + "\n";
                    exceptionMessage += GetImprovedExceptionInformation(errorOutput, input);
                } else {
                    exceptionMessage = outputExceptionMessage + "\n";
                }
                throw new Exception(exceptionMessage);
            }

            return output;
        }

        private string GetImprovedExceptionInformation(string msg, string source, int margin = 3) {
            var lineMatcher = new Regex(".* in line (?<line>[1-9][0-9]*) .*");
            var lineStr = lineMatcher.Match(msg).Groups["line"]?.Value;
            if (!string.IsNullOrEmpty(lineStr)) {
                var line = int.Parse(lineStr);
                return GetLineContent(source, line, margin) + "\n";
            }
            return "";
        }

        private string GetLineContent(string input, int line, int margin = 3) {
            return input.Replace("\r", "")
                .Split('\n')
                .Select((v, i) => new {v, i})
                .Where(e => Math.Abs(e.i - (line - 1)) <= margin)
                .Select(e => (e.i == line - 1 ? "-> " : "   ") + e.v)
                .Aggregate((v1, v2) => v1 + "\n" + v2);
        }
    }
}