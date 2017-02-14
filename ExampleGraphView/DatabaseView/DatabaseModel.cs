using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SharpGraph;

namespace ExampleGraphView {
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class Database {
        public Database() {
            Tables = new Dictionary<string, Table>();
            Relations = new Dictionary<Column, Column>();
        }

        public Database Clone() {
            var database = new Database {Name = Name};
            foreach (var table in Tables) {
                var newTable = new Table(database, table.Value.Name);
                database.Tables[table.Key] = newTable;
                foreach (var column in table.Value.Columns) {
                    var newColumn = new Column(newTable, column.Value.Name);
                    newTable.Columns[column.Key] = newColumn;
                }
            }
            foreach (var relation in Relations) {
                var sourceTable = database.Tables.Values.Single(t => t.Name == relation.Key.Parent.Name);
                var sourceColumn = sourceTable.Columns.Values.Single(c => c.Name == relation.Key.Name);
                var targetTable = database.Tables.Values.Single(t => t.Name == relation.Value.Parent.Name);
                var targetColumn = targetTable.Columns.Values.Single(c => c.Name == relation.Value.Name);
                database.Relations[sourceColumn] = targetColumn;
            }
            return database;
        }

        public Database RelationColumnsOnly() {
            var database = Clone();
            var relationColumns = database.Relations.Keys.Union(database.Relations.Values).ToList();
            var tables = database.Tables.Values.ToList();
            foreach (var table in tables) {
                var nonRelationColumnPairs = table.Columns.Where(c => !relationColumns.Contains(c.Value)).ToList();
                foreach (var p in nonRelationColumnPairs) {
                    table.Columns.Remove(p);
                }
            }
            return database;
        }

        public IGraph ToGraph() {
            var graph = Graph.CreateGraph(Name);
            var graphAttrs = new AttributeDictionary {{"label", Name}};
            graph.SetAttributes(graphAttrs);

            foreach (var p in Tables.Values) {
                var sg = graph.CreateSubGraph("cluster" + p.Name);
                var subGraphAttrs = new AttributeDictionary {{"label", '"' + p.Name + '"'}};
                sg.SetAttributes(subGraphAttrs);
                foreach (var c in p.Columns.Values) {
                    var node = sg.CreateNode(c.Id);
                    var nodeAttrs = new AttributeDictionary {{"label", '"' + c.Name + '"'}};
                    node.SetAttributes(nodeAttrs);
                }
            }
            var nodes = graph.GetNodes().ToList();
            foreach (var p in Relations) {
                var sourceNode = nodes.Single(n => n.Id == p.Key.Id);
                var endNode = nodes.Single(n => n.Id == p.Value.Id);
                graph.CreateEdge(sourceNode, endNode);
            }

            return graph;
        }

        public IDictionary<string, Table> Tables { get; }
        public IDictionary<Column, Column> Relations { get; }
        public string Name { get; set; }

        public override string ToString() {
            return Name;
        }
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class Table {
        public Table(Database parentDatabase, string name) {
            Columns = new Dictionary<string, Column>();
            ParentDatabase = parentDatabase;
            Name = name;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Database ParentDatabase { get; set; }
        public IDictionary<string, Column> Columns { get; }
        public string Name { get; }

        public override string ToString() {
            return Name;
        }
    }

    public class Column {
        public Column(Table parent, string name) {
            Parent = parent;
            Name = name;
        }

        public Table Parent { get; }
        public string Name { get; }
        public string Id => '"' + Parent.ToString() + "." + Name + '"';

        public override string ToString() {
            return Name;
        }
    }
}