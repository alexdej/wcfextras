namespace Microsoft.ServiceModel.Extras
{
    using System;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel;
    using System.Collections.Generic;
    using System.ServiceModel.Description;
    using System.ServiceModel.Channels;

    public class EasyClient : ChannelFactory
    {
        readonly string address;
        ContractDescription contract;

        public EasyClient(string address)
        {
            this.address = address;
            this.InitializeEndpoint((string)null, null);
        }

        protected override ServiceEndpoint CreateDescription()
        {
            contract = new ContractDescription("service", "http://tempuri.org/");

            var uri = new Uri(this.address);

            return new ServiceEndpoint(contract, EasyBindings.GetDefaultBinding(uri.Scheme), new EndpointAddress(uri));
        }

        public object Invoke(string p, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}