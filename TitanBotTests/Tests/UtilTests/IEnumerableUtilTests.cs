using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TitanBotBaseTest.Tests.UtilTests
{
    [TestClass]
    public class IEnumerableUtilTests
    {
        [TestMethod]
        public void TestMask1()
        {
            var expected = new List<bool[]>
            {
                new bool[]{true},
                new bool[]{false}
            };
            var actual = IEnumerableUtil.BooleanMask(1).ToList();
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.IsTrue(actual.Zip(expected, (a, e) => a.Zip(e, (av, ev) => av == ev).All(v => v)).All(v => v));
        }
        [TestMethod]
        public void TestMask2()
        {
            var expected = new List<bool[]>
            {
                new bool[]{true,true},
                new bool[]{true,false},
                new bool[]{false,true},
                new bool[]{false,false},
            };
            var actual = IEnumerableUtil.BooleanMask(2).ToList();
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.IsTrue(actual.Zip(expected, (a, e) => a.Zip(e, (av, ev) => av == ev).All(v => v)).All(v => v));
        }
        [TestMethod]
        public void TestMask3()
        {
            var expected = new List<bool[]>
            {
                new bool[]{true,true,true},
                new bool[]{true,true,false},
                new bool[]{true,false,true},
                new bool[]{true,false,false},
                new bool[]{false,true,true},
                new bool[]{false,true,false},
                new bool[]{false,false,true},
                new bool[]{false,false,false},
            };
            var actual = IEnumerableUtil.BooleanMask(3).ToList();
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.IsTrue(actual.Zip(expected, (a, e) => a.Zip(e, (av, ev) => av == ev).All(v => v)).All(v => v));
        }
    }
}
