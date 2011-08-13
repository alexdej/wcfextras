namespace Microsoft.ServiceModel.Extras
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Collections.Generic;
    using System.Collections;

    public static partial class ServiceDescriptionExtensionMethods
    {
        public static IEnumerable<ContractDescription> AllContracts(this ServiceDescription description)
        {
            return description.Endpoints.Select(e => e.Contract).Distinct();
        }

        public static IEnumerable<OperationDescription> AllOperations(this ServiceDescription description)
        {
            return description.AllContracts().SelectMany(c => c.Operations);
        }

        public static IEnumerable<MessageDescription> AllMessages(this ServiceDescription description)
        {
            return description.AllOperations().SelectMany(o => o.Messages);
        }

        public static IEnumerable<Binding> AllBindings(this ServiceDescription description)
        {
            return description.Endpoints.Select(e => e.Binding);
        }

        // useful for getting everything into one WSDL doc
        public static void UnifyNamespaces(this ServiceDescription description)
        {
            // 1. first validate whether alignment is possible: all contracts need to be in the same namespace

            var namespaces = description.AllContracts().Select(c => c.Namespace);
            if (namespaces.Count() > 1)
            {
                throw new InvalidOperationException("all the contracts in the description need to have the same Namespace");
            }

            // 2. assuming there's a common contract namespace, promote it up to the ServiceDescription
            description.Namespace = namespaces.First();

            // 3. then copy it over to each endpoint's binding
            foreach (var endpoint in description.Endpoints)
            {
                endpoint.Binding.Namespace = endpoint.Contract.Namespace;
            }
        }
    }

    // CONSIDER, alexdej: this could be Dictionary<object, object>
    public class ServiceDescriptionStateBag : Dictionary<string, object>, IServiceBehavior, IContractBehavior, IEndpointBehavior, IOperationBehavior
    {
        public static ServiceDescriptionStateBag InstallTo<T>(KeyedByTypeCollection<T> behaviors)
        {
            var bag = behaviors.Find<ServiceDescriptionStateBag>();
            if (bag == null)
            {
                bag = new ServiceDescriptionStateBag();
                ((IList)behaviors).Add(bag);
            }
            return bag;
        }

        #region IServiceBehavior Members
        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) {}
        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }
        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }
        #endregion

        #region IContractBehavior Members
        void IContractBehavior.AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
        void IContractBehavior.ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime) { }
        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime) {}
        void IContractBehavior.Validate(ContractDescription contractDescription, ServiceEndpoint endpoint){}
        #endregion

        #region IEndpointBehavior Members
        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters){}
        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime){}
        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher){}
        void IEndpointBehavior.Validate(ServiceEndpoint endpoint){}
        #endregion

        #region IOperationBehavior Members
        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters){}
        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation){}
        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation){}
        void IOperationBehavior.Validate(OperationDescription operationDescription){}
        #endregion
    }

}