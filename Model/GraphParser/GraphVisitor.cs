using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DotParser;

namespace SharpGraph {
    [CLSCompliant(false)]
    public class GraphVisitor : DotGrammarBaseVisitor<IGraph> {
        private IGraph ParsedGraph { get; set; }
        private ISubGraph CurrentSubGraph { get; set; }

        public override IGraph VisitGraph(DotGrammarParser.GraphContext context) {
            if ((context == null) || (ParsedGraph != null)) {
                throw new NotImplementedException();
            }
            var id = context.id()?.GetText() ?? ModelHelper.DefaultGraphId;
            var isDirected = context.DIGRAPH() != null;
            var isStrict = context.STRICT() != null;
            ParsedGraph = Graph.CreateGraph(id, isDirected, isStrict);
            CurrentSubGraph = ParsedGraph;

            ParsedGraph.SetAttributes(GetGraphAttributes(context.stmt_list().stmt()));
            ParsedGraph.SetNodeAttributes(GetNodeAttributes(context.stmt_list().stmt()));
            ParsedGraph.SetEdgeAttributes(GetEdgeAttributes(context.stmt_list().stmt()));

            foreach (var stmtContext in context.stmt_list().stmt()) {
                HandleNode(stmtContext.node_stmt());
                HandleEdgeLine(stmtContext.edge_stmt());
                HandleSubgraph(stmtContext.subgraph());
                HandleExitSubgraph(stmtContext.subgraph());
            }

            return ParsedGraph;
        }

        private IList<NodePort> HandleNode(DotGrammarParser.Node_stmtContext context) {
            var nodeList = new List<NodePort>();
            if (context == null) {
                return nodeList;
            }
            var id = context.node_id().id().GetText();
            var portContext = context.node_id().port();
            var port = HandlePort(portContext);
            var attrs = GetAttributes(context.attr_list()?.a_list());
            nodeList.Add(new NodePort(CurrentSubGraph.CreateNode(id, attrs, false), port));
            return nodeList;
        }

        private static IPort HandlePort(DotGrammarParser.PortContext context) {
            if (context == null) {
                return null;
            }
            Compass compass;
            var name = context.id()[0].GetText();
            if (context.id().Length == 1) {
                if (name.ToUpperInvariant() == "_") {
                    return new Port();
                }
                if (Enum.TryParse(name.ToUpperInvariant(), out compass)) {
                    return new Port(compass);
                }
                return new Port(Compass.Default, name);
            }
            var compassStr = context.id()[1].GetText();
            if (!Enum.TryParse(compassStr.ToUpperInvariant(), out compass)) {
                throw new NotImplementedException();
            }
            return new Port(compass, name);
        }

        private IList<NodePort> HandleEdgeLine(DotGrammarParser.Edge_stmtContext context) {
            var nodeList = new List<NodePort>();
            if (context == null) {
                return nodeList;
            }
            var attrs = GetAttributes(context.attr_list()?.a_list());
            var startContext = context.edge_obj();
            var numberOfEdges = context.edgeRHS().edge_obj().Length;
            for (var i = 0; i < numberOfEdges; i++) {
                var endContext = context.edgeRHS().edge_obj()[i];
                //In case we ever need to know the edge type based on the symbol
                //var isDirected = context.edgeRHS().edgeop()[i].GetText() == ModelHelper.DirectedEdgeOpName;
                HandleEdge(startContext, endContext, attrs);
                startContext = endContext;
            }
            return nodeList;
        }

        private void HandleEdge(DotGrammarParser.Edge_objContext startContext,
            DotGrammarParser.Edge_objContext endContext, IAttributeDictionary attrs) {
            IList<NodePort> sourceNodePorts, endNodePorts;
            if (startContext.node_id() != null) {
                var id = startContext.node_id().id().GetText();
                var portContext = startContext.node_id().port();
                var port = HandlePort(portContext);
                sourceNodePorts = new List<NodePort> {
                    new NodePort(CurrentSubGraph.CreateNode(id, checkParent: false), port)
                };
            } else {
                sourceNodePorts = HandleSubgraph(startContext.subgraph()).ToList();
                HandleExitSubgraph(startContext.subgraph());
            }
            if (endContext.node_id() != null) {
                var id = endContext.node_id().id().GetText();
                var portContext = endContext.node_id().port();
                var port = HandlePort(portContext);
                endNodePorts = new List<NodePort> {
                    new NodePort(CurrentSubGraph.CreateNode(id, checkParent: false), port)
                };
            } else {
                endNodePorts = HandleSubgraph(endContext.subgraph()).ToList();
                HandleExitSubgraph(endContext.subgraph());
            }
            foreach (var sourceNodePort in sourceNodePorts) {
                foreach (var endNodePort in endNodePorts) {
                    CurrentSubGraph.CreateEdge(sourceNodePort.Node, endNodePort.Node, attrs, sourceNodePort.Port,
                        endNodePort.Port);
                }
            }
        }

        private IList<NodePort> HandleSubgraph(DotGrammarParser.SubgraphContext context) {
            var nodeList = new List<NodePort>();
            if (context == null) {
                return nodeList;
            }
            var id = context.id() == null
                ? string.Format(CultureInfo.InvariantCulture, ModelHelper.NewSubGraphFormat,
                    ParsedGraph.GetSubGraphs().Count() + 1)
                : context.id().GetText();
            var graphAttrs = GetGraphAttributes(context.stmt_list().stmt());
            var nodeAttrs = GetNodeAttributes(context.stmt_list().stmt());
            var edgeAttrs = GetEdgeAttributes(context.stmt_list().stmt());

            CurrentSubGraph = CurrentSubGraph.CreateSubGraph(id, graphAttrs, nodeAttrs, edgeAttrs);
            foreach (var stmtContext in context.stmt_list().stmt()) {
                nodeList.AddRange(HandleNode(stmtContext.node_stmt()));
                nodeList.AddRange(HandleEdgeLine(stmtContext.edge_stmt()));
                nodeList.AddRange(HandleSubgraph(stmtContext.subgraph()));
                HandleExitSubgraph(stmtContext.subgraph());
            }
            return nodeList;
        }

        private void HandleExitSubgraph(DotGrammarParser.SubgraphContext context) {
            if (context == null) {
                return;
            }
            CurrentSubGraph = CurrentSubGraph.Parent;
        }

        private static IAttributeDictionary GetGraphAttributes(DotGrammarParser.StmtContext[] stmtContext) {
            var attributes = new AttributeDictionary();
            if (stmtContext == null) {
                return attributes;
            }
            foreach (var stmt in stmtContext) {
                if (stmt.attr_stmt()?.GRAPH() != null) {
                    attributes.SetAttributes(GetAttributes(stmt.attr_stmt().attr_list().a_list()));
                } else if ((stmt.id() != null) && (stmt.id().Length > 0)) {
                    var attrKey = ModelHelper.ReduceId(stmt.id()[0].GetText());
                    var attrValue = ModelHelper.ReduceId(stmt.id()[1].GetText());
                    attributes.SetAttributes(new AttributeDictionary {[attrKey] = attrValue});
                }
            }
            return attributes;
        }

        private static IAttributeDictionary GetNodeAttributes(DotGrammarParser.StmtContext[] stmtContext) {
            var attributes = new AttributeDictionary();
            if (stmtContext == null) {
                return attributes;
            }
            foreach (var stmt in stmtContext.Where(s => s.attr_stmt()?.NODE() != null)) {
                attributes.SetAttributes(GetAttributes(stmt.attr_stmt().attr_list().a_list()));
            }
            return attributes;
        }

        private static IAttributeDictionary GetEdgeAttributes(DotGrammarParser.StmtContext[] stmtContext) {
            var attributes = new AttributeDictionary();
            if (stmtContext == null) {
                return attributes;
            }
            foreach (var stmt in stmtContext.Where(s => s.attr_stmt()?.EDGE() != null)) {
                attributes.SetAttributes(GetAttributes(stmt.attr_stmt().attr_list().a_list()));
            }
            return attributes;
        }

        private static IAttributeDictionary GetAttributes(DotGrammarParser.A_listContext[] aListContexts) {
            var attrs = new AttributeDictionary();
            if (aListContexts == null) {
                return attrs;
            }

            foreach (var aListContext in aListContexts) {
                var numberOfAttributes = aListContext.sattr_stmt().Length;
                for (var i = 0; i < numberOfAttributes; i++) {
                    var sAttrIds = aListContext.sattr_stmt()[i].id();
                    var attrKey = sAttrIds[0].GetText();
                    var attrValue = sAttrIds.Length > 1 ? sAttrIds[1].GetText() : null;
                    attrs[attrKey] = attrValue;
                }
            }

            return attrs;
        }

        private class NodePort : Tuple<INode, IPort> {
            public NodePort(INode node, IPort port) : base(node, port) {}
            public INode Node => Item1;
            public IPort Port => Item2;
        }
    }
}