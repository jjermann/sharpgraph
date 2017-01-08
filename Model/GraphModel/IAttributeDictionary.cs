using System.Collections.Generic;

namespace SharpGraph {
    public interface IAttributeDictionary : IDictionary<string, string> {
        void SetAttributes(IAttributeDictionary dict);
    }
}