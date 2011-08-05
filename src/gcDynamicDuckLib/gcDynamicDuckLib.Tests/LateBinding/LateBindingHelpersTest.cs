using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gcDynamicTest
{
    public class Person : IPerson
    {
        public string FavoriteFood { get; set; }

        public string SecretWord
        {
            get
            {
                return "Brodie";
            }
        }

        public string Name { get; set; }

        public int Age { get; set; }

        public void IncrementAge()
        {
            Age++;
        }



        public int SayHello(int message)
        {
            throw new NotImplementedException();
        }

        public string SayHello(string message)
        {
            throw new NotImplementedException();
        }


        public int SayHello2(int message)
        {
            throw new NotImplementedException();
        }

        string IPerson.SecretWord { get; set; }
    }


    /// <summary>
    ///This is a test class for LateBindingHelpersTest and is intended
    ///to contain all LateBindingHelpersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LateBindingHelpersTest
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


        private static void PerformTest(Action action, int limit, Stopwatch stopwatch, string debugFormat)
        {
            stopwatch.Start();
            for (int i = 0; i < limit; i++)
            {
                action();
            }
            stopwatch.Stop();
            string line = String.Format(debugFormat, stopwatch.ElapsedMilliseconds.ToString());
            Debug.WriteLine(line);
            stopwatch.Reset();
        }


        //[TestMethod()]
        //public void SpeedTest()
        //{
        //    IDuckProvider lateBinding = new LateBindingInteractionProvider() { CachePoints = true };
        //    IDuckProvider reflectionBinding = new ReflectionInteractionProvider() { CachePoints = true };
        //    // create objects
        //    var dynamicMan = ProxyFactory.DuckInterface<IPerson>(new Man(), lateBinding);
        //    var reflectionMan = ProxyFactory.DuckInterface<IPerson>(new Man(), reflectionBinding);
        //    var realMan = new Person();
        //    var thunkMan = GeniusCode.Framework.Support.Dynamic.DynamicDuck.Create<IPerson>();
        //    var mockMan = ProxyFactory.MockInterface<IPerson>();
        //    dynamic expandoMan = new ExpandoObject();
        //    // number to retry
        //    int limit = 80000;
        //    Stopwatch stopwatch = new Stopwatch();

        //    // action to apply to interface
        //    Action<IPerson> personAction = p =>
        //        {

        //            p.Name = "Ryan";
        //            var name = p.Name;
        //            p.Age = 5;
        //            var age = p.Age;
        //        };

        //    // set up initial values
        //    personAction(dynamicMan);
        //    personAction(reflectionMan);

        //    // duplicate action for dynamic
        //    Action<dynamic> personActionDynamic = p =>
        //    {

        //        p.Name = "Ryan";
        //        var name = p.Name;
        //        p.Age = 5;
        //        var age = p.Age;
        //    };

        //    // duplication duck action, without the castle part
        //    Action<IDuckProvider, IPerson> personActionWithoutCastle = (dp,p) =>
        //        {
        //            dp.PerformPropertySet("Ryan", p, "Name");
        //            var name = dp.PerformPropertyGet<string>(p, "Name");
        //            dp.PerformPropertySet(5, p, "Age");
        //            dp.PerformPropertyGet<int>(p, "Age");
        //        };




        //    // duplicate action for reflection
        //    var nameProperty = new RelayReflectedProperty<Person, string>("Name", realMan, System.Reflection.BindingFlags.Default, null, null);
        //    var ageProperty = new RelayReflectedProperty<Person, int>("Age", realMan, System.Reflection.BindingFlags.Default, null, null);
        //    Action personActionReflection = () =>
        //        {
        //            nameProperty.SetValue("Ryan");
        //            var name = nameProperty.GetValue();

        //            ageProperty.SetValue(5);
        //            var age = ageProperty.GetValue();
        //        };

        //    // perform tests
        //    Debug.WriteLine("*************  Simple Code  ***************** ");
        //    PerformTest(() => personAction(realMan), limit, stopwatch, "actual code - {0}");
        //    Debug.WriteLine("*************  Mock Technique  ***************** ");
        //    PerformTest(() => personAction(thunkMan), limit, stopwatch, "thunk code - {0}");
        //    PerformTest(() => personAction(mockMan), limit, stopwatch, "mock (castle) code - {0}");
        //    Debug.WriteLine("*************  Static Binding  ***************** ");
        //    PerformTest(() => personActionDynamic(realMan), limit, stopwatch, "dynamic - {0}");
        //    PerformTest(() => personActionReflection(), limit, stopwatch, "reflection - {0}");
        //    Debug.WriteLine("*************  Dynamic Binding  ***************** ");
        //    PerformTest(() => personActionWithoutCastle(lateBinding, realMan), limit, stopwatch, "latebinding w dict - {0}");
        //    PerformTest(() => personActionWithoutCastle(reflectionBinding, realMan), limit, stopwatch, "reflection w dict - {0}");
        //    lateBinding.CachePoints = false;
        //    reflectionBinding.CachePoints = false;
        //    PerformTest(() => personActionWithoutCastle(lateBinding, realMan), limit, stopwatch, "latebinding w/out dict - {0}");
        //    PerformTest(() => personActionWithoutCastle(reflectionBinding, realMan), limit, stopwatch, "reflection w/out dict - {0}");
        //    Debug.WriteLine("*************  Castle Proxies  ***************** ");
        //    lateBinding.CachePoints = true;
        //    reflectionBinding.CachePoints = true;
        //    PerformTest(() => personAction(dynamicMan), limit, stopwatch, "latebinding w dict - {0}");
        //    PerformTest(() => personAction(reflectionMan), limit, stopwatch, "reflection w dict - {0}");
        //    lateBinding.CachePoints = false;
        //    reflectionBinding.CachePoints = false;
        //    PerformTest(() => personAction(dynamicMan), limit, stopwatch, "latebinding w/out dict - {0}");
        //    PerformTest(() => personAction(reflectionMan), limit, stopwatch, "reflection w/out dict - {0}");
             


        //}

    }
}
