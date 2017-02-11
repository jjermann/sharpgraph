using System.Collections.Generic;
using System.Linq;
using SharpGraph;

namespace ExampleGraphView {
    public class Database {
        public Database() {
            Tables = new Dictionary<string, Table>();
            Constraints = new Dictionary<Column, Column>();
        }

        public IGraph ToGraph() {
            var graph = Graph.CreateGraph(Name);
            var graphAttrs = new AttributeDictionary {{"label", Name}};
            graph.SetAttributes(graphAttrs);

            foreach (var p in Tables.Values) {
                var sg = graph.CreateSubGraph("cluster" + p.Name);
                var subGraphAttrs = new AttributeDictionary {{"label", p.Name}};
                sg.SetAttributes(subGraphAttrs);
                foreach (var c in p.Columns.Values) {
                    var node = sg.CreateNode(c.Id);
                    var nodeAttrs = new AttributeDictionary {{"label", c.Name}};
                    node.SetAttributes(nodeAttrs);
                }
            }
            var nodes = graph.GetNodes().ToList();
            foreach (var p in Constraints) {
                var sourceNode = nodes.Single(n => n.Id == p.Key.Id);
                var endNode = nodes.Single(n => n.Id == p.Value.Id);
                graph.CreateEdge(sourceNode, endNode);
            }

            return graph;
        }

        public IDictionary<string, Table> Tables { get; }
        public IDictionary<Column, Column> Constraints { get; }
        // ReSharper disable once MemberCanBePrivate.Global
        public string Name { get; set; }

        public override string ToString() {
            return Name;
        }
    }

    public class Table {
        public Table() {
            Columns = new Dictionary<string, Column>();
        }

        public IDictionary<string, Column> Columns { get; }
        public string Name { get; set; }

        public override string ToString() {
            return Name;
        }
    }

    public class Column {
        // ReSharper disable once MemberCanBePrivate.Global
        public Table Parent { get; set; }
        public string Name { get; set; }
        public string Id => '"' + Parent.ToString() + "." + Name + '"';

        public override string ToString() {
            return Name;
        }
    }
}