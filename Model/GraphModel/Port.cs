using System;

namespace SharpGraph {
    public class Port : IPort {
        public Port(Compass compass = Compass.Default, string name = null) {
            Name = name;
            Compass = compass;
        }

        public string Name { get; }
        public Compass Compass { get; }

        public override string ToString() {
            var compassStr = Compass == Compass.Default ? @"_" : Compass.ToString().ToLowerInvariant();
            if ((Name != null) && (Compass == Compass.Default)) {
                return FormattableString.Invariant($"{Name}");
            }
            if (Name == null) {
                return compassStr;
            }
            return FormattableString.Invariant($"{Name}:{compassStr}");
        }
    }
}