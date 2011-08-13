using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceModel.Extras;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.ServiceModel.Extras.Tests
{
    [TestClass]
    public class BehaviorsTestClass
    {
        public TestContext TestContext { get; set; }

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
        ServiceHost host;
        
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            host = new ServiceHost(typeof(DummyService));
            host.AddServiceEndpoint(typeof(DummyService), new BasicHttpBinding(), "http://localhost:8080/");
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

        [TestMethod]
        public void ServiceBehaviorEventsIsSingletonPerHost()
        {
            var evt1 = ServiceBehaviorEvents.Install(host);
            var evt2 = ServiceBehaviorEvents.Install(host);
            Assert.AreSame(evt1, evt2);

            var host2 = new ServiceHost(typeof(DummyService));
            var evt3 = ServiceBehaviorEvents.Install(host2);
            Assert.AreNotSame(evt1, evt3);
        }

        [TestMethod]
        public void ServiceBehaviorRaisesAllEvents()
        {
            var events = ServiceBehaviorEvents.Install(this.host);

            bool addBindingParameters = false, applyDispatchBehavior = false, validate = false;
            events.AddBindingParameters += delegate { addBindingParameters = true; };
            events.ApplyDispatchBehavior += delegate { applyDispatchBehavior = true; };
            events.Validate += delegate { validate = true; };
            
            this.host.Open();

            Assert.IsTrue(addBindingParameters);
            Assert.IsTrue(applyDispatchBehavior);
            Assert.IsTrue(validate);
        }

        [TestMethod]
        public void EndpointBehaviorEventsIsSingletonPerEndpoint()
        {
            var endpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(DummyService)));
            var evt1 = EndpointBehaviorEvents.Install(endpoint);
            var evt2 = EndpointBehaviorEvents.Install(endpoint);
            Assert.AreSame(evt1, evt2);

            var endpoint2 = new ServiceEndpoint(ContractDescription.GetContract(typeof(DummyService)));
            var evt3 = EndpointBehaviorEvents.Install(endpoint2);
            Assert.AreNotSame(evt1, evt3);
        }

        [TestMethod]
        public void EndpointBehaviorRaisesAllEventsServerSide()
        {
            var events = EndpointBehaviorEvents.Install(host.Description.Endpoints[0]);

            bool addBindingParameters = false, applyDispatchBehavior = false, applyClientBehavior = false, validate = false;
            events.AddBindingParameters += delegate { addBindingParameters = true; };
            events.ApplyDispatchBehavior += delegate { applyDispatchBehavior = true; };
            events.ApplyClientBehavior += delegate { applyClientBehavior = true; };
            events.Validate += delegate { validate = true; };
            
            this.host.Open();

            Assert.IsTrue(addBindingParameters);
            Assert.IsTrue(applyDispatchBehavior);
            Assert.IsFalse(applyClientBehavior);
            Assert.IsTrue(validate);
        }

    }

    [ServiceContract]
    public class DummyService
    {
        [OperationContract]
        public string HelloWorld() { return "Hello World!"; }
    }

    [ServiceContract(Name="DummyService")]
    public interface IDummyService
    {
        [OperationContract]
        string HelloWorld();
    }
}
