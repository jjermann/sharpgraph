using System;
using System.Collections.Generic;

namespace SharpGraph.GraphModel {
    [Serializable]
    public sealed class AttributeDictionary : Dictionary<string, string>, IAttributeDictionary {
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
                    var valuePart = attr.Value != null ? $" = {attr.Value}" : "";
                    output += $"{firstPart}{attr.Key}{valuePart}";
                    firstPart = ", ";
                }
                output += " ]";
            }
            return output;
        }
    }
}