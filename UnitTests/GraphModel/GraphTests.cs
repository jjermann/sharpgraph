using System.Linq;
using NUnit.Framework;

namespace SharpGraph.GraphModel {
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
    }
}