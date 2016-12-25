using System;
using System.Collections.Generic;

namespace SharpGraph.GraphModel {
    public interface ISubGraph : IBaseObject {
        INode CreateNode(
            string id,
            IAttributeDictionary attrs = null,
            bool checkParent = true);

        IEdge CreateEdge(
            INode sourceNode,
            INode endNode,
            IAttributeDictionary attrs = null);

        ISubGraph CreateSubGraph(
            string id,
            IAttributeDictionary graphAttrs = null,
            IAttributeDictionary nodeAttrs = null,
            IAttributeDictionary edgeAttrs = null);

        void SetNodeAttributes(IAttributeDictionary attrs);
        bool HasNodeAttribute(string attr);
        string GetNodeAttribute(string attr);
        IAttributeDictionary GetNodeAttributes();
        void SetEdgeAttributes(IAttributeDictionary attrs);
        bool HasEdgeAttribute(string attr);
        string GetEdgeAttribute(string attr);
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