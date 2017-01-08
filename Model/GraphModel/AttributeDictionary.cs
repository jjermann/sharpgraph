using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharpGraph.GraphModel {
    [Serializable]
    public class AttributeDictionary : Dictionary<string, string>, IAttributeDictionary {
        public AttributeDictionary() {}
        protected AttributeDictionary(SerializationInfo info, StreamingContext context) : base(info, context) {}

        public void SetAttributes(IAttributeDictionary dict) {
            if (dict == null) {
                return;
            }
            foreach (var attr in dict) {
                this[ModelHelper.ReduceId(attr.Key)] = ModelHelper.ReduceId(attr.Value);
            }
        }

        public override string ToString() {
            return ToDot();
        }

        private string ToDot() {
            var output = "";
            if (Count > 0) {
                output += "[ ";
                var firstPart = "";
                foreach (var attr in this) {
                    var valuePart = attr.Value != null ? FormattableString.Invariant($" = {attr.Value}") : "";
                    output += FormattableString.Invariant($"{firstPart}{attr.Key}{valuePart}");
                    firstPart = ", ";
                }
                output += " ]";
            }
            return output;
        }
    }
}