using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.Diagnostics;
using System.IO;

namespace Microsoft.ServiceModel.Extras.Tests
{
    [TestClass]
    public class DiagnosticsTests
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
        public void EnableTracing()
        {
            using (var w = new StringWriter())
            {
                using (var l = new TextWriterTraceListener(w))
                {
                    host.TraceTo(l, SourceLevels.Information, WcfTraceSource.ServiceModel);
                    host.Open();
                }
                var contents = w.ToString();
                Assert.IsTrue(contents.Contains("Endpoint listener opened"));
                TestContext.WriteLine(contents);
            }
        }

        [TestMethod]
        public void EnableTracingToDefault()
        {
            host.TraceTo(new DefaultTraceListener(), SourceLevels.Information, WcfTraceSource.ServiceModel);
            host.Open();

            // manually verify that traces were delivered to the debug console
        }
    }
}
