using System.Linq;
using NUnit.Framework;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphParser {
    [TestFixture]
    public static class DotOutputTests {
        [SetUp]
        public static void Init() {}

        private static void CheckIdConsistencies(IGraph graph) {
            Assert.IsFalse(graph.GetNodes().GroupBy(n => n.Id).Any(c => c.Count() > 1));
            Assert.IsFalse(graph.GetSubGraphs().GroupBy(s => s.Id).Any(c => c.Count() > 1));
            Assert.IsFalse(graph.GetEdges().GroupBy(e => e.Id).Any(c => c.Count() > 1));
        }

        [Test]
        public static void DigraphTypeTest() {
            var input = TestHelper.ExampleDot.Replace("digraph {", "digraph {");
            var graph = GraphParser.GetGraph(input);
            Assert.IsTrue(graph.IsDirected);
            Assert.IsFalse(graph.IsStrict);
            Assert.IsTrue(graph.Id == ModelHelper.DefaultGraphId);
            CheckIdConsistencies(graph);
            Assert.IsTrue(graph.GetEdges().GroupBy(e => new {e.SourceNode, e.EndNode}).Any(c => c.Count() > 1));
            var output = graph.ToDot();
            Assert.IsTrue(output.StartsWith("digraph GraphId {"));
        }

        [Test]
        public static void DuplicateNodesTest() {
            var input = "A\nA";
            var graph = GraphParser.GetGraph($"digraph {{\n{input}\n}}");
            var output = graph.ToDot();
            var expectedOutput = "digraph GraphId {\n  A\n}\n";
            Assert.AreEqual(expectedOutput, output);

            input = "A->B\nA";
            graph = GraphParser.GetGraph($"digraph {{\n{input}\n}}");
            output = graph.ToDot();
            expectedOutput = "digraph GraphId {\n  A -> B\n}\n";
            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public static void EmptyGraphTest() {
            var input = "";
            var graph = GraphParser.GetGraph($"digraph {{\n{input}\n}}");
            Assert.IsTrue(!graph.GetNodes().Any());
            Assert.IsTrue(!graph.GetEdges().Any());
            Assert.IsTrue(!graph.GetSubGraphs().Any());
            Assert.IsTrue(!graph.GetAttributes().Any());
            Assert.IsTrue(!graph.GetNodeAttributes().Any());
            Assert.IsTrue(!graph.GetEdgeAttributes().Any());

            var output = graph.ToDot();
            var expectedOutput = "digraph GraphId {\n}\n";
            Assert.AreEqual(expectedOutput, output);
        }

        [Test]
        public static void GraphTypeTest() {
            var input = TestHelper.ExampleDot.Replace("digraph {", "graph MyId {");
            var graph = GraphParser.GetGraph(input);
            Assert.IsFalse(graph.IsDirected);
            Assert.IsFalse(graph.IsStrict);
            Assert.IsTrue(graph.Id == "MyId");
            CheckIdConsistencies(graph);
            Assert.IsTrue(graph.GetEdges().GroupBy(e => new {e.SourceNode, e.EndNode}).Any(c => c.Count() > 1));
            var output = graph.ToDot();
            Assert.IsTrue(output.StartsWith("graph MyId {"));
        }

        [Test]
        public static void StrictTypeTest() {
            var input = TestHelper.ExampleDot.Replace("digraph {", "strict digraph MyId {");
            var graph = GraphParser.GetGraph(input);
            Assert.IsTrue(graph.IsDirected);
            Assert.IsTrue(graph.IsStrict);
            Assert.IsTrue(graph.Id == "MyId");
            CheckIdConsistencies(graph);
            Assert.IsFalse(graph.GetEdges().GroupBy(e => new {e.SourceNode, e.EndNode}).Any(c => c.Count() > 1));
            Assert.IsTrue(graph.GetEdges().All(e => e.Id == "node" + e.SourceNode.Id + e.EndNode.Id));
            var output = graph.ToDot();
            Assert.IsTrue(output.StartsWith("strict digraph MyId {"));
        }
    }
}