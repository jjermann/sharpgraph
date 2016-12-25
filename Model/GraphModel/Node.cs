namespace SharpGraph.GraphModel {
    public class Node : BaseObject, INode {
        public Node(ISubGraph parentGraph, string id) : base(parentGraph, id) {}

        public override string ToString() {
            var output = Id;
            if (Attributes.Count > 0) {
                output += " " + Attributes;
            }
            return output;
        }
    }
}