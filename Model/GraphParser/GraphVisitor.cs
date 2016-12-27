using System;
using System.Collections.Generic;
using System.Linq;
using DotParser;
using SharpGraph.GraphModel;

namespace SharpGraph.GraphParser {
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

        private IList<INode> HandleNode(DotGrammarParser.Node_stmtContext context) {
            var nodeList = new List<INode>();
            if (context == null) {
                return nodeList;
            }
            var id = context.node_id().id().GetText();
            var attrs = GetAttributes(context.attr_list()?.a_list());
            nodeList.Add(CurrentSubGraph.CreateNode(id, attrs, false));
            return nodeList;
        }

        private IList<INode> HandleEdgeLine(DotGrammarParser.Edge_stmtContext context) {
            var nodeList = new List<INode>();
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
            IList<INode> sourceNodes, endNodes;
            if (startContext.node_id() != null) {
                var id = startContext.node_id().id().GetText();
                sourceNodes = new List<INode> {CurrentSubGraph.CreateNode(id, checkParent: false)};
            } else {
                sourceNodes = HandleSubgraph(startContext.subgraph()).ToList();
                HandleExitSubgraph(startContext.subgraph());
            }
            if (endContext.node_id() != null) {
                var id = endContext.node_id().id().GetText();
                endNodes = new List<INode> {CurrentSubGraph.CreateNode(id, checkParent: false)};
            } else {
                endNodes = HandleSubgraph(endContext.subgraph()).ToList();
                HandleExitSubgraph(endContext.subgraph());
            }
            foreach (var sourceNode in sourceNodes) {
                foreach (var endNode in endNodes) {
                    CurrentSubGraph.CreateEdge(sourceNode, endNode, attrs);
                }
            }
        }

        private IList<INode> HandleSubgraph(DotGrammarParser.SubgraphContext context) {
            var nodeList = new List<INode>();
            if (context == null) {
                return nodeList;
            }
            var id = context.id() == null
                ? string.Format(ModelHelper.NewSubGraphFormat, ParsedGraph.GetSubGraphs().Count() + 1)
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

        private IAttributeDictionary GetGraphAttributes(DotGrammarParser.StmtContext[] stmtContext) {
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

        private IAttributeDictionary GetNodeAttributes(DotGrammarParser.StmtContext[] stmtContext) {
            var attributes = new AttributeDictionary();
            if (stmtContext == null) {
                return attributes;
            }
            foreach (var stmt in stmtContext.Where(s => s.attr_stmt()?.NODE() != null)) {
                attributes.SetAttributes(GetAttributes(stmt.attr_stmt().attr_list().a_list()));
            }
            return attributes;
        }

        private IAttributeDictionary GetEdgeAttributes(DotGrammarParser.StmtContext[] stmtContext) {
            var attributes = new AttributeDictionary();
            if (stmtContext == null) {
                return attributes;
            }
            foreach (var stmt in stmtContext.Where(s => s.attr_stmt()?.EDGE() != null)) {
                attributes.SetAttributes(GetAttributes(stmt.attr_stmt().attr_list().a_list()));
            }
            return attributes;
        }

        private IAttributeDictionary GetAttributes(DotGrammarParser.A_listContext[] aListContexts) {
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
    }
}