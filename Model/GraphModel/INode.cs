using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpGraph {
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public interface INode : IBaseObject {
        IEnumerable<IEdge> IncomingEdges();
        IEnumerable<IEdge> OutgoingEdges();
        IEnumerable<IEdge> ConnectedEdges();
        IEnumerable<INode> IncomingNeighbours();
        IEnumerable<INode> OutgoingNeighbours();
        IEnumerable<INode> ConnectedNeighbours();

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void VisitNeighbours(Func<INode, bool> stopCondition, IDictionary<INode, HashSet<INode>> visited);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        HashSet<INode> RecursiveSelect(Func<INode, IEnumerable<INode>> selectionFunc);
    }
}