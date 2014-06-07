using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Controls;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetDistanceTest()
        {
            var st1 = new PointView(0, 0);
            var st2 = new PointView(3, 4);
            Assert.AreEqual(5, st1.GetDistanse(st2), 0.001);

        }
    }
}
