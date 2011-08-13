namespace Microsoft.ServiceModel.Extras
{
    using System;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Collections.Generic;
    using System.Linq;


    public static partial class ServiceHostExtensionMethods
    {
        #region IServiceBehavior

        // accelerator methods for IServiceBehavior, directly on ServiceHost
        public static void ApplyDispatchBehavior(this ServiceHostBase host, Action<ServiceDescription, ServiceHostBase> callback)
        {
            ServiceBehaviorEvents.Install(host).ApplyDispatchBehavior += callback;
        }
        public static void AddBindingParameters(this ServiceHostBase host, Action<ServiceDescription, ServiceHostBase, Collection<ServiceEndpoint>, BindingParameterCollection> callback)
        {
            ServiceBehaviorEvents.Install(host).AddBindingParameters += callback;
        }
        public static void Validate(this ServiceHostBase host, Action<ServiceDescription, ServiceHostBase> callback)
        {
            ServiceBehaviorEvents.Install(host).Validate += callback;
        }
        #endregion

        #region IEndpointBehavior
        // accelerator methods for IEndpointBehavior, directly on ServiceHost
        // each method can take either a single endpoint or an IEnumerable<ServiceEndpoint> (eg result of a query)
        public static void ApplyDispatchBehavior(this ServiceHostBase host, ServiceEndpoint endpoint, Action<ServiceEndpoint, EndpointDispatcher> callback)
        {
            EndpointBehaviorEvents.Install(endpoint).ApplyDispatchBehavior += callback;
        }
        public static void ApplyDispatchBehavior(this ServiceHostBase host, IEnumerable<ServiceEndpoint> endpoints, Action<ServiceEndpoint, EndpointDispatcher> callback)
        {
            foreach (var endpoint in endpoints)
            {
                EndpointBehaviorEvents.Install(endpoint).ApplyDispatchBehavior += callback;
            }
        }

        public static void AddBindingParameters(this ServiceHostBase host, ServiceEndpoint endpoint, Action<ServiceEndpoint, BindingParameterCollection> callback)
        {
            EndpointBehaviorEvents.Install(endpoint).AddBindingParameters += callback;
        }
        public static void AddBindingParameters(this ServiceHostBase host, IEnumerable<ServiceEndpoint> endpoints, Action<ServiceEndpoint, BindingParameterCollection> callback)
        {
            foreach (var endpoint in endpoints)
            {
                EndpointBehaviorEvents.Install(endpoint).AddBindingParameters += callback;
            }
        }

        public static void Validate(this ServiceHostBase host, ServiceEndpoint endpoint, Action<ServiceEndpoint> callback)
        {
            EndpointBehaviorEvents.Install(endpoint).Validate += callback;
        }
        public static void Validate(this ServiceHostBase host, IEnumerable<ServiceEndpoint> endpoints, Action<ServiceEndpoint> callback)
        {
            foreach (var endpoint in endpoints)
            {
                EndpointBehaviorEvents.Install(endpoint).Validate += callback;
            }
        }

        #endregion

        #region IContractBehavior
        // accelerator methods for IContractBehavior, directly on ServiceHost
        // each method can take either a single operation or an IEnumerable<ContractDescription> (eg result of a query)
        public static void ApplyDispatchBehavior(this ServiceHostBase host, ContractDescription contract, Action<ContractDescription, ServiceEndpoint, DispatchRuntime> callback)
        {
            ContractBehaviorEvents.Install(contract).ApplyDispatchBehavior += callback;
        }
        public static void ApplyDispatchBehavior(this ServiceHostBase host, IEnumerable<ContractDescription> contracts, Action<ContractDescription, ServiceEndpoint, DispatchRuntime> callback)
        {
            foreach (var contract in contracts)
            {
                ContractBehaviorEvents.Install(contract).ApplyDispatchBehavior += callback;
            }
        }

        public static void AddBindingParameters(this ServiceHostBase host, ContractDescription contract, Action<ContractDescription, ServiceEndpoint, BindingParameterCollection> callback)
        {
            ContractBehaviorEvents.Install(contract).AddBindingParameters += callback;
        }
        public static void AddBindingParameters(this ServiceHostBase host, IEnumerable<ContractDescription> contracts, Action<ContractDescription, ServiceEndpoint, BindingParameterCollection> callback)
        {
            foreach (var contract in contracts)
            {
                ContractBehaviorEvents.Install(contract).AddBindingParameters += callback;
            }
        }

        public static void Validate(this ServiceHostBase host, ContractDescription contract, Action<ContractDescription, ServiceEndpoint> callback)
        {
            ContractBehaviorEvents.Install(contract).Validate += callback;
        }
        public static void Validate(this ServiceHostBase host, IEnumerable<ContractDescription> contracts, Action<ContractDescription, ServiceEndpoint> callback)
        {
            foreach (var contract in contracts)
            {
                ContractBehaviorEvents.Install(contract).Validate += callback;
            }
        }
        #endregion 

        #region IOperationBehavior

        // accelerator methods for IOperationBehavior, directly on ServiceHost
        // each method can take either a single operation or an IEnumerable<OperationDescription> (eg result of a query)
        public static void ApplyDispatchBehavior(this ServiceHostBase host, OperationDescription operation, Action<OperationDescription, DispatchOperation> callback)
        {
            OperationBehaviorEvents.Install(operation).ApplyDispatchBehavior += callback;
        }
        public static void ApplyDispatchBehavior(this ServiceHostBase host, IEnumerable<OperationDescription> operations, Action<OperationDescription, DispatchOperation> callback)
        {
            foreach (var operation in operations)
            {
                OperationBehaviorEvents.Install(operation).ApplyDispatchBehavior += callback;
            }
        }

        public static void AddBindingParameters(this ServiceHostBase host, OperationDescription operation, Action<OperationDescription, BindingParameterCollection> callback)
        {
            OperationBehaviorEvents.Install(operation).AddBindingParameters += callback;
        }
        public static void AddBindingParameters(this ServiceHostBase host, IEnumerable<OperationDescription> operations, Action<OperationDescription, BindingParameterCollection> callback)
        {
            foreach (var operation in operations)
            {
                OperationBehaviorEvents.Install(operation).AddBindingParameters += callback;
            }
        }

        public static void Validate(this ServiceHostBase host, OperationDescription operation, Action<OperationDescription> callback)
        {
            OperationBehaviorEvents.Install(operation).Validate += callback;
        }
        public static void Validate(this ServiceHostBase host, IEnumerable<OperationDescription> operations, Action<OperationDescription> callback)
        {
            foreach (var operation in operations)
            {
                OperationBehaviorEvents.Install(operation).Validate += callback;
            }
        }
        #endregion
    }

    public static class KeyedByTypeCollectionExtensionMethods
    {
        public static T Ensure<T>(this KeyedByTypeCollection<IServiceBehavior> behaviors)
            where T : IServiceBehavior, new()
        {
            return behaviors.Ensure<IServiceBehavior, T>();
        }

        public static T Ensure<T>(this KeyedByTypeCollection<IEndpointBehavior> behaviors)
            where T : IEndpointBehavior, new()
        {
            return behaviors.Ensure<IEndpointBehavior, T>();
        }

        public static T Ensure<T>(this KeyedByTypeCollection<IContractBehavior> behaviors)
            where T : IContractBehavior, new()
        {
            return behaviors.Ensure<IContractBehavior, T>();
        }

        public static T Ensure<T>(this KeyedByTypeCollection<IOperationBehavior> behaviors)
            where T : IOperationBehavior, new()
        {
            return behaviors.Ensure<IOperationBehavior, T>();
        }

        // TODO, alexdej: this isn't really usable on its own as the TCollection argument doesn't bind automatically
        // ie you have to specify both parameters: description.Behaviors.Ensure<IServiceBehavior, ServiceDebugBehavior>()
        // so i have the overloads above, which is fine i guess
        public static TItem Ensure<TCollection, TItem>(this KeyedByTypeCollection<TCollection> collection)
            where TItem : TCollection, new()
        {
            TItem item = collection.Find<TItem>();
            if (item == null)
            {
                item = new TItem();
                collection.Add(item);
            }
            return item;
        }
    }

    public class ServiceBehaviorEvents : IServiceBehavior
    {
        public event Action<ServiceDescription, ServiceHostBase> ApplyDispatchBehavior;
        public event Action<ServiceDescription, ServiceHostBase, Collection<ServiceEndpoint>, BindingParameterCollection> AddBindingParameters;
        public event Action<ServiceDescription, ServiceHostBase> Validate;

        public static ServiceBehaviorEvents Install(ServiceHostBase host)
        {
            var evt = host.Description.Behaviors.Find<ServiceBehaviorEvents>();
            if (evt == null)
            {
                evt = new ServiceBehaviorEvents();
                host.Description.Behaviors.Add(evt);
            }
            return evt;
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            var evt = this.AddBindingParameters;
            if (evt != null)
            {
                evt(serviceDescription, serviceHostBase, endpoints, bindingParameters);
            }
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var evt = this.ApplyDispatchBehavior;
            if (evt != null)
            {
                evt(serviceDescription, serviceHostBase);
            }
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var evt = this.Validate;
            if (evt != null)
            {
                evt(serviceDescription, serviceHostBase);
            }
        }
    }

    public class EndpointBehaviorEvents : IEndpointBehavior
    {
        public event Action<ServiceEndpoint, BindingParameterCollection> AddBindingParameters;
        public event Action<ServiceEndpoint, ClientRuntime> ApplyClientBehavior;
        public event Action<ServiceEndpoint, EndpointDispatcher> ApplyDispatchBehavior;
        public event Action<ServiceEndpoint> Validate;

        public static EndpointBehaviorEvents Install(ServiceEndpoint endpoint)
        {
            var evt = endpoint.Behaviors.Find<EndpointBehaviorEvents>();
            if (evt == null)
            {
                evt = new EndpointBehaviorEvents();
                endpoint.Behaviors.Add(evt);
            }
            return evt;
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            var evt = this.AddBindingParameters;
            if (evt != null)
            {
                evt(endpoint, bindingParameters);
            }
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var evt = this.ApplyClientBehavior;
            if (evt != null)
            {
                evt(endpoint, clientRuntime);
            }
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var evt = this.ApplyDispatchBehavior;
            if (evt != null)
            {
                evt(endpoint, endpointDispatcher);
            }
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
            var evt = this.Validate;
            if (evt != null)
            {
                evt(endpoint);
            }
        }
    }

    public class ContractBehaviorEvents : IContractBehavior
    {
        public event Action<ContractDescription, ServiceEndpoint, BindingParameterCollection> AddBindingParameters;
        public event Action<ContractDescription, ServiceEndpoint, ClientRuntime> ApplyClientBehavior;
        public event Action<ContractDescription, ServiceEndpoint, DispatchRuntime> ApplyDispatchBehavior;
        public event Action<ContractDescription, ServiceEndpoint> Validate;

        public static ContractBehaviorEvents Install(ContractDescription contract)
        {
            var evt = contract.Behaviors.Find<ContractBehaviorEvents>();
            if (evt == null)
            {
                evt = new ContractBehaviorEvents();
                contract.Behaviors.Add(evt);
            }
            return evt;
        }

        void IContractBehavior.AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            var evt = this.AddBindingParameters;
            if (evt != null)
            {
                evt(contractDescription, endpoint, bindingParameters);
            }
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var evt = this.ApplyClientBehavior;
            if (evt != null)
            {
                evt(contractDescription, endpoint, clientRuntime);
            }
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            var evt = this.ApplyDispatchBehavior;
            if (evt != null)
            {
                evt(contractDescription, endpoint, dispatchRuntime);
            }
        }

        void IContractBehavior.Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
            var evt = this.Validate;
            if (evt != null)
            {
                evt(contractDescription, endpoint);
            }
        }
    }


    public class OperationBehaviorEvents : IOperationBehavior
    {
        public event Action<OperationDescription, BindingParameterCollection> AddBindingParameters;
        public event Action<OperationDescription, ClientOperation> ApplyClientBehavior;
        public event Action<OperationDescription, DispatchOperation> ApplyDispatchBehavior;
        public event Action<OperationDescription> Validate;

        public static OperationBehaviorEvents Install(OperationDescription operation)
        {
            var evt = operation.Behaviors.Find<OperationBehaviorEvents>();
            if (evt == null)
            {
                evt = new OperationBehaviorEvents();
                operation.Behaviors.Add(evt);
            }
            return evt;
        }

        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
            var evt = this.AddBindingParameters;
            if (evt != null)
            {
                evt(operationDescription, bindingParameters);
            }
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            var evt = this.ApplyClientBehavior;
            if (evt != null)
            {
                evt(operationDescription, clientOperation);
            }
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            var evt = this.ApplyDispatchBehavior;
            if (evt != null)
            {
                evt(operationDescription, dispatchOperation);
            }
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
            var evt = this.Validate;
            if (evt != null)
            {
                evt(operationDescription);
            }
        }
    }
}