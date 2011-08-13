namespace Microsoft.ServiceModel.Extras
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Description;

    public static partial class ServiceHostExtensionMethods
    {
        // returns all the EndpointDispatchers associated with a particular contract by name and namespace
        // if you have a ContractDescription, you can get the name and namespace using .Name and .Namespace
        // if you have a [ServiceContract] type you can get the ContractDescription using ContractDescription.GetContract
        public static IEnumerable<EndpointDispatcher> GetEndpointDispatchersForContract(this ServiceHostBase host, string contractName, string contractNamespace)
        {
            return host.GetEndpointDispatchers().Where((e) => e.ContractName == contractName && e.ContractNamespace == contractNamespace);
        }

        public static IEnumerable<EndpointDispatcher> GetEndpointDispatchers(this ServiceHostBase host)
        {
            return host.GetChannelDispachers().SelectMany((c) => c.Endpoints);
        }

        // not sure how to implement this
        //public static EndpointDispatcher GetEndpointDispatcher(this ServiceHostBase host, ServiceEndpoint endpoint)
        //{
        //}

        public static IEnumerable<ChannelDispatcher> GetChannelDispachers(this ServiceHostBase host)
        {
            return host.ChannelDispatchers.OfType<ChannelDispatcher>();
        }

        public static void OnEndpointDispatcherAvailable(this ServiceHostBase host, Action<ServiceEndpoint, EndpointDispatcher> callback)
        {
            host.ApplyDispatchBehavior(host.Description.Endpoints, callback);
        }
    }
}