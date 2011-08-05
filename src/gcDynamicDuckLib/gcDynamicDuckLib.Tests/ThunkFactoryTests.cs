using System;
using System.ComponentModel;
using System.Dynamic;
using GeniusCode.Components.DynamicDuck;
using GeniusCode.Components.DynamicDuck.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gcDynamicDuckLib.Tests
{


    [TestClass()]
    public class ThunkFactoryTests
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


        [TestMethod]
        public void Should_Cache_Thunk_Types()
        {
            var dip = new DictionaryInteractionProvider();
            var expando = new ExpandoObject();

            var tf = new ThunkFactory(dip);

            IDogOwner ryan;

            for (int i = 0; i < 100; i++)
            {
                ryan = tf.AsIf<IDogOwner>(expando, true);
            }

            Assert.AreEqual(1, tf.GenerateCount);

        }




        [TestMethod]
        public void Should_Duck_Expando_As_Interface()
        {
            var mp = new DictionaryInteractionProvider();
            var dyn = new ExpandoObject();
            var tf = new ThunkFactory(mp);
            IDogOwner ryan = tf.AsIf<IDogOwner>(dyn, true);

            tf.AsIf<IDogOwner>(dyn, true);
            tf.LastAssemblyBuilder.Save(@"Ryan.dll");

            bool itHappened = false;
            (ryan as INotifyPropertyChanged).PropertyChanged += (s, e) => itHappened = true;

            Assert.AreEqual(ryan.Dogname, "Brodie");

            //Doesn't work... needs to be implemented still
            //ryan.HaveBirthday += (s, e) => ryan.Age++;
            ryan.Age = 100;
            ryan.DogNickname = "Mr.Man";
            ryan.value = "something";
            IPerson<string> r2 = ryan as IPerson<string>;
            Assert.IsNotNull(r2);
            Assert.AreEqual(r2.Age, ryan.Age);
            Assert.AreEqual(r2.value, ryan.value);



            var r3 = tf.AsIfRewrap<IPerson<int>>(ryan as IDynamicProxy, true);
            r3.value = 25;
            Assert.AreEqual(r3.Age, ryan.Age);
            Assert.AreNotEqual(r3.value, ryan.value);
            Assert.IsTrue(itHappened);

        }

        public interface IPerson
        {
            event EventHandler<EventArgs> HaveBirthday;
            int Age { get; set; }
        }

        public interface IPerson<T> : IPerson
        {
            T value { get; set; }
        }

        public interface IDogOwner : IPerson<string>
        {
            [DefaultValue("Brodie")]
            string Dogname { get; set; }
            int DogAge { get; set; }
            int LeashLength { get; set; }
            string DogNickname { get; set; }
        }
    }
}

