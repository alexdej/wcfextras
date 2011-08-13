using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.ServiceModel.Extras;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.Net;
using System.ServiceModel.Description;

namespace Microsoft.ServiceModel.Extras.Tests
{
    /// <summary>
    /// Summary description for Metadata
    /// </summary>
    [TestClass]
    public class MetadataTests
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

        const string uri = "http://localhost:8080/";

        ServiceHost host;
        [TestInitialize()]
        public void MyTestInitialize()
        {
            host = new ServiceHost(typeof(DummyService), new Uri(uri));
            host.AddServiceEndpoint(typeof(DummyService), new BasicHttpBinding(), "");
            host.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = true });
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

        const string marker = "CanAlterHelpPage put me here";
        [TestMethod]
        public void CanAlterHelpPage()
        {
            host.AddMetadataMessageInspector(new EasyMetadataInterceptor()
            {
                Html = doc =>
                {
                    var body = doc.Descendants().Where((n) => n.Name.LocalName == "BODY").FirstOrDefault();
                    body.AddFirst(new XElement(XName.Get("p"), marker));
                    return doc;
                },
            });
            host.Open();

            var helppage = new WebClient().DownloadString(uri);
            Assert.IsTrue(helppage.Contains("<p>" + marker + "</p>"));
        }
        [TestMethod]
        public void CanAlterWsdl()
        {
            host.AddMetadataMessageInspector(new EasyMetadataInterceptor()
            {
                Wsdl = doc =>
                {
                    foreach (var port in doc.Descendants().Where((n) => n.Name.LocalName == "port"))
                    {
                        port.AddFirst(new XElement(XName.Get("wsdl"), marker));
                    }
                    return doc;
                },
            });
            host.Open();

            var wsdl = new WebClient().DownloadString(uri + "?WSDL");
            Assert.IsTrue(wsdl.Contains("<wsdl>" + marker + "</wsdl>"));
        }
        //[TestMethod]
        public void CanAlterXsd()
        {
            host.AddMetadataMessageInspector(new EasyMetadataInterceptor()
            {
                Xsd = doc =>
                {
                    var someType = doc.Descendants().Where((n) => n.Name.LocalName == "complexType").FirstOrDefault();
                    if (someType != null)
                    {
                        someType.AddFirst(EasyMetadataInterceptor.CreateDocumentationAnnotation(marker, "en"));
                    }
                    return doc;
                },
            });
            host.Open();

            var xsd = new WebClient().DownloadString(uri + "?xsd=xsd0");
            Assert.IsTrue(xsd.Contains(marker));
        }

        [TestMethod]
        public void InlineXsd()
        {
            host.InlineXsdsInWsdl();
            host.Open();

            var wsdl = new WebClient().DownloadString(uri + "?WSDL");
            Assert.IsTrue(wsdl.Contains(":schema"));
            
            // there are a number of things we want to check here such as whether the dummy import namespace was deleted
            // this Assert serves as a proxy: check that the default DC serialization schema got imported
            Assert.IsTrue(wsdl.Contains("http://schemas.microsoft.com/2003/10/Serialization/"));
        }
    }
}
