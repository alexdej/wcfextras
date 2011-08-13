using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Extras.Tests
{
    [TestClass]
    public class BindingsTests
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

        [TestMethod]
        public void CommonProperty()
        {
            var host = new ServiceHost(typeof(DummyService), new Uri("http://localhost:8080/"), new Uri("net.tcp://localhost:8080/"));
            var http = new BasicHttpBinding();
            var tcp = new NetTcpBinding();
            host.AddServiceEndpoint(typeof(DummyService), http, "");
            host.AddServiceEndpoint(typeof(DummyService), tcp, "");
            host.Description.AllBindings().CommonProperties().MaxReceivedMessageSize = 1024 * 1024 * 1024;

            Assert.AreEqual(1024 * 1024 * 1024, http.MaxReceivedMessageSize);
            Assert.AreEqual(1024 * 1024 * 1024, tcp.MaxReceivedMessageSize);
        }

        [TestMethod]
        public void CommonHttpProperty()
        {
            var host = new ServiceHost(typeof(DummyService), new Uri("http://localhost:8080/"), new Uri("net.tcp://localhost:8080/"));
            var http = new BasicHttpBinding();
            var http2 = new WSHttpBinding();
            var tcp = new NetTcpBinding();
            var proxyAddress = new Uri("http://dummy/");
            host.AddServiceEndpoint(typeof(DummyService), http, "one");
            host.AddServiceEndpoint(typeof(DummyService), http2, "two");
            host.AddServiceEndpoint(typeof(DummyService), tcp, "three");
            host.Description.AllBindings().CommonHttpProperties().ProxyAddress = proxyAddress;

            Assert.AreEqual(proxyAddress, http.ProxyAddress);
            Assert.AreEqual(proxyAddress, http2.ProxyAddress);
        }

        [TestMethod]
        public void CommonConnectionProperty()
        {
            var host = new ServiceHost(typeof(DummyService), new Uri("http://localhost:8080/"), new Uri("net.tcp://localhost:8080/"), new Uri("net.pipe://localhost/"));
            var http = new BasicHttpBinding();
            var tcp = new NetTcpBinding();
            var pipe = new NetNamedPipeBinding();
            var proxyAddress = new Uri("http://dummy/");
            host.AddServiceEndpoint(typeof(DummyService), http, "");
            host.AddServiceEndpoint(typeof(DummyService), tcp, "");
            host.AddServiceEndpoint(typeof(DummyService), pipe, "");
            host.Description.AllBindings().CommonConnectionProperties().MaxConnections = 999;

            Assert.AreEqual(999, tcp.MaxConnections);
            Assert.AreEqual(999, pipe.MaxConnections);
        }

    }
}
