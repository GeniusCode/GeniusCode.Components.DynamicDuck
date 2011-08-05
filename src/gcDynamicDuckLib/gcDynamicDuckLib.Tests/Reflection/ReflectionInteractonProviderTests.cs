using GeniusCode.Components.DynamicDuck;
using GeniusCode.Components.DynamicDuck.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gcDynamicDuckLib.Tests
{
    /// <summary>
    /// Summary description for ReflectionInteractonProviderTests
    /// </summary>
    [TestClass]
    public class ReflectionInteractonProviderTests
    {
        public ReflectionInteractonProviderTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            string card = "12312312312312312";

            var bob = new ClassWithPrivateStuff(card, "Bob");

            var rip = new ReflectionInteractionProvider();

            var tp = new ThunkFactory(rip);


            var bob2 = tp.AsIf<IFindPrivateStuff>(bob, false);


            bob2._CreditCardNumber = "You've been hacked!";



            var authorized = bob.ValidateCreditCard(card);


            Assert.IsFalse(authorized);


            authorized = bob.ValidateCreditCard("You've been hacked!");

            Assert.IsTrue(authorized);


            bob2.EmptyWallet();

            authorized = bob.ValidateCreditCard("");

            Assert.IsTrue(authorized);

        }



        public class Person
        {
        }
    }


    public interface IFindPrivateStuff
    {
        string _CreditCardNumber { get; set; }
        void EmptyWallet();
    }

    public class ClassWithPrivateStuff
    {
        public ClassWithPrivateStuff(string creditCardNumber, string Name)
        {
            _CreditCardNumber = creditCardNumber;
        }


        public bool ValidateCreditCard(string number)
        {
            return number == _CreditCardNumber;
        }

        private string _CreditCardNumber { get; set; }


        private void EmptyWallet()
        {
            _CreditCardNumber = "";
        }
    }

}
