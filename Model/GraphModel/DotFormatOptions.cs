using System;

namespace SharpGraph {
    public class DotFormatOptions {
        public bool OrderByName { get; set; } = false;
        public bool ShowRedundantNodes { get; set; } = false;
        public bool BodyOnly { get; set; } = false;
        public Func<INode, bool> NodeSelector { get; set; } = null;

        public DotFormatOptions Clone() {
            return (DotFormatOptions) MemberwiseClone();
        }
    }
}