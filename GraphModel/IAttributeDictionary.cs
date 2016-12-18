using System.Collections.Generic;

namespace SharpGraph.GraphModel {
    public interface IAttributeDictionary : IDictionary<string, string> {
        void SetAttributes(IAttributeDictionary dict);
    }
}