using System;

namespace SharpGraph.GraphModel {
    public interface IBaseObject : IEquatable<IBaseObject>, IComparable<IBaseObject> {
        ISubGraph Parent { get; }
        string Id { get; }
        int SubGraphDepth { get; }
        IGraph Root { get; }
        void SetAttributes(IAttributeDictionary attrs);
        bool HasAttribute(string attr, bool recursive = false);
        string GetAttribute(string attr, bool recursive = false);
        IAttributeDictionary GetAttributes();
    }
}