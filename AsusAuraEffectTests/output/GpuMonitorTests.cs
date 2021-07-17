using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsusAuraEffect.monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsusAuraEffect.Tests
{
    [TestClass()]
    public class GpuMonitorTests
    {
        [TestMethod()]
        public void GetGpuTemperatureTest()
        {
            using(var util = new GpuMonitor())
            {
                util.Open(); ;
                var ret = util.GetValue();

                Console.WriteLine(ret);

                Assert.IsNotNull(ret);
            }
        }
    }
}