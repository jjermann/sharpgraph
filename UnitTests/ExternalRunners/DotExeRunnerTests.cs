using System;
using System.Drawing;
using NUnit.Framework;

namespace SharpGraph.ExternalRunners {
    [TestFixture]
    public static class DotExeRunnerTests {
        [SetUp]
        public static void Init() {}

        [Test]
        public static void GetGraphLayoutTest() {
            try {
                new DotExeRunner<string>("-Tdot", reader => reader.ReadToEnd())
                    .GetOutput(TestHelper.ExampleDot);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
            try {
                new DotExeRunner<Image>("-Tpng", reader => Image.FromStream(reader.BaseStream))
                    .GetOutput(TestHelper.ExampleDot);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }
    }
}