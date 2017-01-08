using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpGraph {
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
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