using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Extras.Tests
{
    [TestClass]
    public class EasyServiceTests
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
        //
        #endregion

        EasyService service;

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() 
        {
            if (service != null)
            {
                service.Abort();
                service = null;
            }
        }

        [TestMethod]
        public void EasyService()
        {
            var service = new EasyService("MyFirstService")
                .Debuggable()
                .At("http://localhost:8080/")
                .WithOperation("HelloWorld")
                    .Calls((string name) => "Hello " + name);
            
            using (service)
            {
                service.Open();

                using (var cf = new ChannelFactory<IMyFirstService>(new BasicHttpBinding(), "http://localhost:8080/"))
                {
                    var result = cf.CreateChannel().HelloWorld("alex");
                    Assert.AreEqual("Hello alex", result);
                }

                service.Close();
            }
        }
    }

    [ServiceContract(Name="MyFirstService")]
    public interface IMyFirstService
    {
        [OperationContract]
        string HelloWorld(string name);
    }

}
