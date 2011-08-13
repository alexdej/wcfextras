using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.ServiceModel.Description;

using Microsoft.ServiceModel.Extras;
using System.Runtime.Serialization;

namespace Microsoft.ServiceModel.Extras.Tests
{
    /// <summary>
    /// Summary description for SerializationTests
    /// </summary>
    [TestClass]
    public class SerializationTests
    {
        public TestContext TestContext { get; set; }

        ServiceHost host;
        [TestInitialize()]
        public void MyTestInitialize()
        {
            host = new ServiceHost(typeof(DummyService));
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


        [TestMethod]
        public void WhenHostHasMetadataGetXsdDataContractExporterReturnsValue()
        {
            host.Description.Behaviors.Add(new ServiceMetadataBehavior());

            Assert.IsNotNull(host.GetXsdDataContractExporter());
        }

        [TestMethod]
        public void WhenHostDoesntHaveMetadataGetXsdDataContractExporterReturnsNull()
        {
            Assert.IsNull(host.GetXsdDataContractExporter());
        }

        [TestMethod]
        public void ExistingOptionsNotClobbered()
        {
            var exporter = new XsdDataContractExporter() { Options = new ExportOptions() };
            var smb = new ServiceMetadataBehavior();

            smb.MetadataExporter.State.Add(exporter.GetType(), exporter);
            host.Description.Behaviors.Add(smb);

            var exporter2 = host.GetXsdDataContractExporter();
            Assert.ReferenceEquals(exporter, exporter2);
            Assert.ReferenceEquals(exporter.Options, exporter2.Options);
        }

        //[TestMethod]
        public void ReuseExistingSchemaSet()
        {
            // TODO
        }
        //[TestMethod]
        public void MetadataImporterNotAWsdlImporter()
        {
            // TODO
        }
    }
}
