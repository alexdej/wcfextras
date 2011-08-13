using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ServiceModel.Extras;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.ServiceModel.Extras.Tests
{
    [TestClass]
    public class DispatcherTests
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
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        ServiceHost host;
        [TestInitialize()]
        public void MyTestInitialize()
        {
            host = new ServiceHost(typeof(DummyService));
            host.AddServiceEndpoint(typeof(DummyService), new BasicHttpBinding(), "http://localhost:8080/");
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (host != null)
            {
                host.Close();
                host = null;
            }
        }

        //[TestMethod]
        public void ApplyDispatchBehaviorCanChangeDispatchRuntime()
        {
            // TODO: ideally need some way to troll the endpoints
            host.ApplyDispatchBehavior(delegate
            {
                //host.GetEndpointDispatcher(host.Description.Endpoints[0]).
            });
        }

        [TestMethod]
        public void OnEndpointDispatcherAvailableCanChangeDispatchRuntime()
        {
            host.OnEndpointDispatcherAvailable((endpoint, dispatcher) =>
            {
                Assert.IsTrue(dispatcher.DispatchRuntime.SuppressAuditFailure);
                dispatcher.DispatchRuntime.SuppressAuditFailure = false;
            });

            host.Open();

            List<EndpointDispatcher> dispatchers = new List<EndpointDispatcher>(host.GetEndpointDispatchers());
            Assert.IsFalse(dispatchers[0].DispatchRuntime.SuppressAuditFailure);
        }

        [TestMethod]
        public void CanChangeDispatchOperationViaApplyDispatchBehavior()
        {
            host.ApplyDispatchBehavior(
                host.Description.AllOperations()
                    .Where(o => o.Name == "HelloWorld" && o.DeclaringContract.ContractType == typeof(DummyService)),
                (operation, dispatcher) =>
                {
                    Assert.IsTrue(dispatcher.AutoDisposeParameters);
                    dispatcher.AutoDisposeParameters = false;
                });

            host.Open();

            Assert.IsFalse(((ChannelDispatcher)host.ChannelDispatchers[0]).Endpoints[0].DispatchRuntime.Operations["HelloWorld"].AutoDisposeParameters);
        }

        [TestMethod]
        public void OneMethodService()
        {
            host.ApplyDispatchBehavior(
                host.Description.AllOperations()
                    .Where(o => o.Name == "HelloWorld" && o.DeclaringContract.ContractType == typeof(DummyService)),
                (operation, dispatcher) =>
                {
                    dispatcher.Invoker = EasyMethodInvoker.Invoke((object o) => "Hello World Intercepted!");
                });

            host.Open();

            using (var cf = new ChannelFactory<IDummyService>(new BasicHttpBinding(), "http://localhost:8080/"))
            {
                var result = cf.CreateChannel().HelloWorld();
                Assert.AreEqual(result, "Hello World Intercepted!");
            }
        }

        [TestMethod]
        public void FunWithDelegates()
        {
            Func<object, object> del = obj => "Hello World" + obj;

            // what can we discover?

            // declared method parameters are taken from Func<>
            Assert.AreEqual("arg", del.GetType().GetMethod("Invoke").GetParameters()[0].Name); 

            // but if we peek into the target Method ...
            Assert.AreEqual("obj", del.Method.GetParameters()[0].Name);
        }

    }
}
