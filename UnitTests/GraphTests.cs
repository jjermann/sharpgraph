using System.Linq;
using NUnit.Framework;

namespace SharpGraph {
    [TestFixture]
    public static class GraphTests {
        [SetUp]
        public static void Init() {}

        [Test]
        public static void CreateEmptyGraphTest() {
            var graph = Graph.CreateGraph("MyGraph");
            Assert.IsTrue(!graph.GetNodes().Any());
            Assert.IsTrue(!graph.GetEdges().Any());
            Assert.IsTrue(!graph.GetSubGraphs().Any());
            Assert.IsTrue(!graph.GetAttributes().Any());
            Assert.IsTrue(!graph.GetNodeAttributes().Any());
            Assert.IsTrue(!graph.GetEdgeAttributes().Any());
        }

        [Test]
        public static void SpecialCharacterIdsTest() {
            var id = GetLongId();
            var input = "digraph " + id + " { A; }";
            var graph = GraphParser.GetGraph(input);
            Assert.AreEqual(id, graph.Id);
            //TODO: This should be true but apparently graphviz does not support its own grammar...
            //Assert.IsTrue(GraphParser.SyntaxChecker(input));
        }

        [Test]
        public static void NoLeadingNumberTest() {
            var id = '1' + GetLongId();
            var input = "digraph " + id + " { A; }";
            var graph = GraphParser.GetGraph(input);
            Assert.AreNotEqual(id, graph.Id);
            Assert.Catch(() => GraphParser.SyntaxChecker(input));
        }

        private static string GetLongId() {
            var id = "";
            for (var c = 'a'; c <= 'z'; c++) {
                id += c;
            }
            for (var c = 'A'; c <= 'Z'; c++) {
                id += c;
            }
            for (var c = '0'; c <= '9'; c++) {
                id += c;
            }
            for (var c = '0'; c <= '9'; c++) {
                id += c;
            }
            for (var c = '\u0080'; c <= '\u00FF'; c++) {
                id += c;
            }
            id += '_';
            return id;
        }
    }
}