using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Extras.Tests
{
    [TestClass]
    public class EasyClientTests
    {
        public TestContext TestContext { get; set; }

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

        #endregion

        ServiceHost host;

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            host = new ServiceHost(typeof(MyFirstService));
            host.AddServiceEndpoint(typeof(MyFirstService), new BasicHttpBinding(), "http://localhost:8080/");
        }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (host != null)
            {
                host.Close();
                host = null;
            }
        }

        // EasyClient isn't implemented yet
        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public void EasyClient()
        {
            var client = new EasyClient("http://localhost:8080");
            client.Invoke("HelloWorld", "Alex");
        }
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ServiceContract]
    public class MyFirstService
    {
        [OperationContract]
        public string HelloWorld(string name)
        {
            return "Hello " + name;
        }
    }
}
