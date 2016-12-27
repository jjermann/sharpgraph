namespace SharpGraph.GraphModel {
    public class Node : BaseObject, INode {
        public Node(ISubGraph parentGraph, string id) : base(parentGraph, id) {}

        public override bool HasAttribute(string attr, bool recursive = false) {
            var hasLocalAttr = Attributes.ContainsKey(attr);
            if (!recursive || hasLocalAttr || (Parent == null)) {
                return hasLocalAttr;
            }
            return Parent.HasNodeAttribute(attr, true);
        }

        //Warning: This function will throw an exception in case the attribute doesn't exist!
        public override string GetAttribute(string attr, bool recursive = false) {
            if (!recursive || (Parent == null) || Attributes.ContainsKey(attr)) {
                return Attributes[attr];
            }
            return Parent.GetNodeAttribute(attr, true);
        }

        public override string ToString() {
            var output = Id;
            if (Attributes.Count > 0) {
                output += " " + Attributes;
            }
            return output;
        }
    }
}