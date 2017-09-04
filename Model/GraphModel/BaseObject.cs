using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpGraph {
    [SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes")]
    public abstract class BaseObject : IBaseObject {
        protected BaseObject(ISubGraph parentGraph, string id) {
            Parent = parentGraph;
            Id = ModelHelper.ReduceId(id);
            Attributes = new AttributeDictionary();
        }

        protected IAttributeDictionary Attributes { get; }

        public virtual void SetAttributes(IAttributeDictionary attrs) {
            Attributes.SetAttributes(attrs);
        }

        public virtual bool HasAttribute(string attr, bool recursive = false) {
            var hasLocalAttr = Attributes.ContainsKey(attr);
            if (!recursive || hasLocalAttr || (Parent == null)) {
                return hasLocalAttr;
            }
            return Parent.HasAttribute(attr, true);
        }

        //Warning: This function will throw an exception in case the attribute doesn't exist!
        public virtual string GetAttribute(string attr, bool recursive = false) {
            if (!recursive || (Parent == null) || Attributes.ContainsKey(attr)) {
                return Attributes[attr];
            }
            return Parent.GetAttribute(attr, true);
        }

        public virtual IAttributeDictionary GetAttributes() {
            var attrs = new AttributeDictionary();
            foreach (var attr in Attributes) {
                attrs[attr.Key] = attr.Value;
            }
            return attrs;
        }

        public ISubGraph Parent { get; }
        public virtual string Id { get; }

        public virtual int SubGraphDepth {
            get {
                var depth = 0;
                var root = (IBaseObject) this;
                while (root?.Parent != null) {
                    root = root.Parent;
                    depth += 1;
                }
                return depth;
            }
        }

        public virtual IGraph Root {
            get {
                var root = Parent;
                while (root.Parent != null) {
                    root = root.Parent;
                }
                return root as IGraph;
            }
        }

        public virtual bool Equals(IBaseObject other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id);
            //return Equals(Parent, other.Parent) && string.Equals(Id, other.Id);
        }

        public virtual int CompareTo(IBaseObject other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            if (Parent != other.Parent) {
                if (Parent == null) {
                    return -1;
                }
                if (other.Parent == null) {
                    return 1;
                }
                return Parent.CompareTo(other.Parent);
            }
            return string.Compare(Id, other.Id, StringComparison.Ordinal);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as IBaseObject;
            return (other != null) && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return Id?.GetHashCode() ?? 0;
                //return ((Parent?.GetHashCode() ?? 0) * 397) ^ (Id?.GetHashCode() ?? 0);
            }
        }
    }
}