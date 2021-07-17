using AsusAuraEffect.output;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsusAuraEffect.Tests
{
    [TestClass()]
    public class AsusAuraOutputTests
    {
        [TestMethod()]
        public void ConvertColorTest_Success()
        {
            Assert.AreEqual((uint)0x00000000, AsusAuraOutput.ConvertColor(0, 0, 0));
            Assert.AreEqual((uint)0x000000ff, AsusAuraOutput.ConvertColor(255, 0, 0));
            Assert.AreEqual((uint)0x0000ff00, AsusAuraOutput.ConvertColor(0, 255, 0));
            Assert.AreEqual((uint)0x00ff0000, AsusAuraOutput.ConvertColor(0, 0, 255));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConvertColorTest_Error_000()
        {
            Assert.AreEqual(0x00000000, AsusAuraOutput.ConvertColor(-1, 0, 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConvertColorTest_Error_001()
        {
            Assert.AreEqual(0x00000000, AsusAuraOutput.ConvertColor(0, -1, 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConvertColorTest_Error_002()
        {
            Assert.AreEqual(0x00000000, AsusAuraOutput.ConvertColor(0, 0 , - 1));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConvertColorTest_Error_010()
        {
            Assert.AreEqual(0x00000000, AsusAuraOutput.ConvertColor(256, 0, 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConvertColorTest_Error_011()
        {
            Assert.AreEqual(0x00000000, AsusAuraOutput.ConvertColor(0, 256, 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConvertColorTest_Error_012()
        {
            Assert.AreEqual(0x00000000, AsusAuraOutput.ConvertColor(0, 0, 256));
        }
    }
}