using System;
using NUnit.Framework;

namespace SharpGraph.ExternalRunners {
    [TestFixture]
    public class DotExeRunnerTests {
        [SetUp]
        public void Init() {}

        [Test]
        public void GetGraphLayoutTest() {
            try {
                new DotExeRunner().GetGraphLayout(TestHelper.ExampleDot);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }
    }
}
