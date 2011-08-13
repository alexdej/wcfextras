using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel.Description;

namespace Microsoft.ServiceModel.Extras.Tests
{
    [TestClass]
    public class DescriptionTests
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

        ServiceDescription service;
        ServiceEndpoint endpoint;
        ContractDescription contract;
        OperationDescription operation;

        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            service = new ServiceDescription();
            contract = new ContractDescription("foo");
            endpoint = new ServiceEndpoint(contract);
            operation = new OperationDescription("hello", contract);

            ServiceDescriptionStateBag.InstallTo(service.Behaviors);
            ServiceDescriptionStateBag.InstallTo(contract.Behaviors);
            ServiceDescriptionStateBag.InstallTo(endpoint.Behaviors);
            ServiceDescriptionStateBag.InstallTo(operation.Behaviors);
        }
        
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() { }
        

        [TestMethod]
        public void StateBagInstall()
        {
            Assert.IsNotNull(service.Behaviors.Find<ServiceDescriptionStateBag>());
            Assert.IsNotNull(contract.Behaviors.Find<ServiceDescriptionStateBag>());
            Assert.IsNotNull(endpoint.Behaviors.Find<ServiceDescriptionStateBag>());
            Assert.IsNotNull(operation.Behaviors.Find<ServiceDescriptionStateBag>());
        }

        [TestMethod]
        public void StateBagGetSet()
        {
            service.Behaviors.Find<ServiceDescriptionStateBag>()["foo"] = 5;
            Assert.AreEqual(service.Behaviors.Find<ServiceDescriptionStateBag>()["foo"], 5);
        }
    }
}
