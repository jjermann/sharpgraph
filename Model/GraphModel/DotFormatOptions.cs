using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpGraph {
    [SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
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