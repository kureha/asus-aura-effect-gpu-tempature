using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsusAuraEffect.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsusAuraEffect.utils.Tests
{
    [TestClass()]
    public class SpectreUtilTests
    {
        [TestMethod()]
        public void SpectreUtilTest_000()
        {
            var util = new SpectreUtil();
            Assert.AreEqual(256 * 5, util.ColorList.Count);
        }

        [TestMethod()]
        public void SpectreUtilTest_Color_000()
        {
            var color = new SpectreUtil.Color
            {
                R = SpectreUtil.MIN_COLOR_VALUE,
                B = SpectreUtil.MIN_COLOR_VALUE,
                G = SpectreUtil.MIN_COLOR_VALUE
            };

            Assert.IsTrue(color.IsValid());

            color.R = SpectreUtil.MAX_COLOR_VALUE;
            color.B = SpectreUtil.MAX_COLOR_VALUE;
            color.G = SpectreUtil.MAX_COLOR_VALUE;

            Assert.IsTrue(color.IsValid());
        }

        [TestMethod()]
        public void SpectreUtilTest_Color_001()
        {
            var color = new SpectreUtil.Color
            {
                R = SpectreUtil.MIN_COLOR_VALUE - 1,
                B = SpectreUtil.MIN_COLOR_VALUE,
                G = SpectreUtil.MIN_COLOR_VALUE
            };

            Assert.IsFalse(color.IsValid());

            color.R = SpectreUtil.MIN_COLOR_VALUE;
            color.B = SpectreUtil.MIN_COLOR_VALUE - 1;
            color.G = SpectreUtil.MIN_COLOR_VALUE;

            Assert.IsFalse(color.IsValid());

            color.R = SpectreUtil.MIN_COLOR_VALUE;
            color.B = SpectreUtil.MIN_COLOR_VALUE;
            color.G = SpectreUtil.MIN_COLOR_VALUE - 1;

            Assert.IsFalse(color.IsValid());
        }

        [TestMethod()]
        public void SpectreUtilTest_Color_002()
        {
            var color = new SpectreUtil.Color
            {
                R = SpectreUtil.MAX_COLOR_VALUE + 1,
                B = SpectreUtil.MAX_COLOR_VALUE,
                G = SpectreUtil.MAX_COLOR_VALUE
            };

            Assert.IsFalse(color.IsValid());

            color.R = SpectreUtil.MAX_COLOR_VALUE;
            color.B = SpectreUtil.MAX_COLOR_VALUE + 1;
            color.G = SpectreUtil.MAX_COLOR_VALUE;

            Assert.IsFalse(color.IsValid());

            color.R = SpectreUtil.MAX_COLOR_VALUE;
            color.B = SpectreUtil.MAX_COLOR_VALUE;
            color.G = SpectreUtil.MAX_COLOR_VALUE + 1;

            Assert.IsFalse(color.IsValid());
        }

        [TestMethod()]
        public void ChangeColorTest_MAX_000()
        {
            var beforeColor = new SpectreUtil.Color() { R = 255, G = 0, B = 0 };
            var afterColor = new SpectreUtil.Color() { R = 0, G = 0, B = 255 };

            var util = new SpectreUtil();
            var colorList = util.ChangeColor(beforeColor, afterColor);

            Assert.AreEqual(256 * 4, colorList.Count);
        }


        [TestMethod()]
        public void ChangeColorTest_Count_000()
        {
            var beforeColor = new SpectreUtil.Color() { R = 255, G = 0, B = 0 };
            var afterColor = new SpectreUtil.Color() { R = 0, G = 0, B = 255 };
            var splitCount = 100;

            var util = new SpectreUtil();
            var colorList = util.ChangeColor(beforeColor, afterColor, splitCount);

            Assert.AreEqual(splitCount, colorList.Count);
            Assert.IsTrue(beforeColor.ColorEquals(colorList[0]));
            Assert.IsTrue(afterColor.ColorEquals(colorList[colorList.Count - 1]));
        }

        [TestMethod()]
        public void ChangeColorTest_Count_001()
        {
            for (int i = 2; i <= 256 * 4; i++)
            {
                Console.WriteLine($"Split number : {i}");

                var beforeColor = new SpectreUtil.Color() { R = 255, G = 0, B = 0 };
                var afterColor = new SpectreUtil.Color() { R = 0, G = 0, B = 255 };
                var splitCount = i;

                var util = new SpectreUtil();
                var colorList = util.ChangeColor(beforeColor, afterColor, splitCount);

                Assert.AreEqual(splitCount, colorList.Count);
                Assert.IsTrue(beforeColor.ColorEquals(colorList[0]));
                Assert.IsTrue(afterColor.ColorEquals(colorList[colorList.Count - 1]));
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ChangeColorTest_Count_Error_000()
        {
            var beforeColor = new SpectreUtil.Color() { R = 255, G = 0, B = 0 };
            var afterColor = new SpectreUtil.Color() { R = 0, G = 0, B = 255 };
            var splitCount = 1;

            var util = new SpectreUtil();
            util.ChangeColor(beforeColor, afterColor, splitCount);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ChangeColorTest_Count_Error_001()
        {
            var beforeColor = new SpectreUtil.Color() { R = 255, G = 0, B = 0 };
            var afterColor = new SpectreUtil.Color() { R = 0, G = 0, B = 255 };
            var splitCount = 256 * 5 + 1;

            var util = new SpectreUtil();
            util.ChangeColor(beforeColor, afterColor, splitCount);
        }

        [TestMethod()]
        public void GetTempatureIndexTest_000()
        {
            var util = new SpectreUtil();

            var minTemp = 0;
            var maxTemp = 100;

            Assert.AreEqual(0, util.GetTempatureIndex(minTemp, minTemp, maxTemp));
            Assert.AreEqual(256 * 5 - 1, util.GetTempatureIndex(maxTemp, minTemp, maxTemp));
        }

        [TestMethod()]
        public void GetTempatureIndexTest_Error_00()
        {
            var util = new SpectreUtil();

            var minTemp = 50;
            var maxTemp = 50;

            Assert.AreEqual(0, util.GetTempatureIndex(minTemp, minTemp, maxTemp));
        }
    }
}