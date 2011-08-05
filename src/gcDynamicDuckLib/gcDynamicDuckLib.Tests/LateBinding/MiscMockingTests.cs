using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using GcCore.Tests.Dynamic;
using GeniusCode.Components.DynamicDuck;
using GeniusCode.Components.DynamicDuck.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeniusCode.Components.DynamicDuck.Providers.LateBinding;

namespace gcDynamicTest
{


    /// <summary>
    ///This is a test class for CastleBasicTest and is intended
    ///to contain all CastleBasicTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MiscMockingTests
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion





        [TestMethod()]
        public void WhatDoesMockMean()
        {
            IPerson p = ProxyFactory.MockInterface<IPerson>();
            var members = p.GetType().GetMembers().ToList();

            members.ForEach(m => Debug.WriteLine(m.ToString()));

        }

        [TestMethod()]
        public void MockInterfaceUsingExpandoProvider()
        {
            TestMockInterface<DictionaryInteractionProvider>(new ExpandoObject());
            TestMockInterface<DictionaryInteractionProvider>(new Dictionary<string, object>());
        }

        private static void TestMockInterface<TProvider>(IDictionary<string, object> target)
            where TProvider : IMockInteractionProvider, new()
        {
            var p = ProxyFactory.MockInterface<IPerson>(new TProvider(), target);

            p.Age = 11;
            p.Name = "Jeremiah";

            Assert.AreEqual(11, p.Age);
            Assert.AreEqual("Jeremiah", p.Name);
        }

        [TestMethod()]
        public void MockInterfaceUsingLateBindingProvider()
        {
            TestMockInterface<LateBindingInteractionProvider>(new ExpandoObject());
        }


        [TestMethod]
        public void Test_Generate_Thunk_ReflectionProvider()
        {
            TestGenerateThunk<ReflectionInteractionProvider>("ReflectionThunk.dll");
        }

        [TestMethod]
        public void Test_Generate_Thunk_LateBindingProvider()
        {
            TestGenerateThunk<LateBindingInteractionProvider>("LateBindingThunk.dll");
        }

        [TestMethod]
        public void Test_Generate_Thunk_DictionaryProvider()
        {
            TestGenerateThunk<DictionaryInteractionProvider>("DictionaryThunk.dll");
        }


        public void TestGenerateThunk<TProvider>(string assemblyName) where TProvider : IDynamicInteractionProvider, new()
        {
            var tf = new ThunkFactory<TProvider>();
            tf.GetThunkType<ITestClass>();
            tf.LastAssemblyBuilder.Save(assemblyName);
        }



        private static void TestDuckInterface<TProvider>()
            where TProvider : IDuckInteractionProvider, new()
        {
            var man = new Man() { Name = "Jeremiah" };

            var asPerson = ProxyFactory.DuckInterface<IPerson>(man, new TProvider());

            asPerson.Age = 5;

            Assert.AreEqual(man.Name, asPerson.Name);
            Assert.AreEqual(5, man.Age);
            // test we can even use private properties
            if (typeof(TProvider) == typeof(ReflectionInteractionProvider))
                Assert.AreEqual("Brodie", asPerson.SecretWord);


            var hello2 = asPerson.SayHello2(11);
            Assert.AreEqual(52, hello2);
        }
        [TestMethod()]
        public void DuckInterfaceUsingReflectionProvider()
        {
            TestDuckInterface<ReflectionInteractionProvider>();
        }


        [TestMethod()]
        public void DuckInterfaceUsingLateBindingProvider()
        {
            TestDuckInterface<LateBindingInteractionProvider>();
        }

        [TestMethod()]
        public void uhoh()
        {
            GcDynamic.Tests.SampleDynamicUsage.Sample();
        }

        [TestMethod()]
        public void TestInvokeSimilarMethods()
        {
            dynamic m = new Man();

            var s = m.SayHello("Hi");
            var i = m.SayHello(5);

            Assert.AreEqual(s, "Hi");
            Assert.AreEqual(i, 5);

            IPerson proxyMan = ProxyFactory.DuckInterface<IPerson>(m, new LateBindingInteractionProvider());

            var s2 = proxyMan.SayHello("Hi");
            var i2 = proxyMan.SayHello(0);

            Assert.AreEqual(s2, "Hi");
            Assert.AreEqual(i2, 5);


        }


        [TestMethod()]
        public void TestInvokeUniqueMethod()
        {
            var m = new Man();
            IPerson proxyMan = ProxyFactory.DuckInterface<IPerson>(m, new LateBindingInteractionProvider());
            var i2 = proxyMan.SayHello2(0);
            Assert.AreEqual(52, i2);
        }


        [TestMethod()]
        public void TestInvokeUniqueMethod_NoProxy()
        {
            var m = new Man();
            var intArgs = new object[] { 0 };
            var invoker = LateBindingHelpers.CreateDynamicMethodInvoker("SayHello", 1);
            object intResult = invoker.Invoke(m, intArgs);

            Assert.AreEqual(5, intResult);


            var stringArgs = new object[] { "Hello" };
            object stringResult = invoker.Invoke(m, stringArgs);

            Assert.AreEqual("Hi", stringResult);
        }

        [TestMethod()]
        public void TestGetProperty()
        {
            var m = new Man() { Age = 6, Name = "Jeremiah" };
            IPerson proxyMan = ProxyFactory.DuckInterface<IPerson>(m, new LateBindingInteractionProvider());
            var i2 = proxyMan.Age;
            Assert.AreEqual(i2, 6);
        }

        [TestMethod()]
        public void TestSetProperty()
        {
            var m = new Man();
            IPerson proxyMan = ProxyFactory.DuckInterface<IPerson>(m, new LateBindingInteractionProvider());
            proxyMan.Age = 6;
            Assert.AreEqual(6, m.Age);
        }

    }
}
