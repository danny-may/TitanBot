using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitanBot.Dependencies;

namespace DiscordBotTest
{
    [TestClass]
    public class DependencyManagerTests
    {
        [TestMethod]
        public void AddAndGetGeneric()
        {
            var expected = new TestObject();

            var manager = new DependencyFactory();
            manager.Store(expected);
            Assert.IsTrue(manager.TryGet(out TestObject actual));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AddAndGetNonGeneric()
        {
            var expected = new TestObject();

            var manager = new DependencyFactory();
            manager.Store(typeof(TestObject), expected);
            Assert.IsTrue(manager.TryGet(typeof(TestObject), out object actual));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AddAndGetInhereted()
        {
            var expected = new TestInheretedObject();

            var manager = new DependencyFactory();
            manager.Store(expected);
            Assert.IsTrue(manager.TryGet(typeof(TestObject), out object actual));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConstructMatching()
        {
            var expected = new TestObject();

            var manager = new DependencyFactory();
            manager.Store(expected);

            if (!manager.TryConstruct(out TestConstructableObject actual))
                Assert.Fail();

            Assert.AreEqual(expected.ID, actual.Child.ID);
        }

        [TestMethod]
        public void ConstructInhereted()
        {
            var expected = new TestInheretedObject();

            var manager = new DependencyFactory();
            manager.Store(expected);

            if (!manager.TryConstruct(out TestConstructableObject actual))
                Assert.Fail();

            Assert.AreEqual(expected.ID, actual.Child.ID);
        }

        [TestMethod]
        public void UnableToConstruct()
        {
            var manager = new DependencyFactory();

            Assert.IsFalse(manager.TryConstruct(out TestConstructableObject actual));
        }

        [TestMethod]
        public void ConstructWithOptionalParams()
        {
            var manager = new DependencyFactory();
            manager.Store(new TestObject());

            Assert.IsTrue(manager.TryConstruct(out TestConstructableObject_Optional obj));
            Assert.AreEqual(13, obj.Var);

            manager.Store(10);
            Assert.IsTrue(manager.TryConstruct(out TestConstructableObject_Optional obj2));
            Assert.AreEqual(10, obj2.Var);
        }

        class TestObject
        {
            private static int counter = 0;
            public int ID { get; }
            public TestObject()
            {
                ID = counter++;
            }
        }
        class TestInheretedObject : TestObject
        {

        }
        class TestConstructableObject
        {
            public TestObject Child { get; }

            public TestConstructableObject(TestObject obj)
            {
                Child = obj;
            }
        }
        class TestConstructableObject_Optional
        {
            public TestObject Child { get; }
            public int Var { get; }

            public TestConstructableObject_Optional(TestObject obj, int var = 13)
            {
                Child = obj;
                Var = var;
            }
        }
    }
}
