using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpGraph {
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface ISubGraph : IBaseObject {
        INode CreateNode(
            string id,
            IAttributeDictionary attrs = null,
            bool checkParent = true);

        IEdge CreateEdge(
            INode sourceNode,
            INode endNode,
            IAttributeDictionary attrs = null,
            IPort sourcePort = null,
            IPort endPort = null);

        ISubGraph CreateSubGraph(
            string id,
            IAttributeDictionary graphAttrs = null,
            IAttributeDictionary nodeAttrs = null,
            IAttributeDictionary edgeAttrs = null);

        void SetNodeAttributes(IAttributeDictionary attrs);
        bool HasNodeAttribute(string attr, bool recursive = false);
        string GetNodeAttribute(string attr, bool recursive = false);
        IAttributeDictionary GetNodeAttributes();
        void SetEdgeAttributes(IAttributeDictionary attrs);
        bool HasEdgeAttribute(string attr, bool recursive = false);
        string GetEdgeAttribute(string attr, bool recursive = false);
        IAttributeDictionary GetEdgeAttributes();

        IEnumerable<INode> GetSubGraphNodes(bool recursive = false);
        IEnumerable<IEdge> GetSubGraphEdges(bool recursive = false);
        IEnumerable<ISubGraph> GetSubGraphSubGraphs(bool recursive = false);

        string ToDot(
            bool orderedByName = true,
            bool showRedundantNodes = false,
            bool bodyOnly = false,
            Func<INode, bool> nodeSelector = null);
    }
}