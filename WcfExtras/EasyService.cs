namespace Microsoft.ServiceModel.Extras
{
    using System;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel;
    using System.Collections.Generic;
    using System.ServiceModel.Description;
    using System.ServiceModel.Channels;

    public class EasyService : ServiceHostBase
    {
        readonly string name;
        readonly string ns;
        ContractDescription contract;

        public EasyService() : this("Service") { }
        public EasyService(string name)
            : base()
        {
            this.name = name;
            this.ns = "http://tempuri.org/";

            this.InitializeDescription(new UriSchemeKeyedCollection());
        }

        protected override ServiceDescription CreateDescription(out IDictionary<string, ContractDescription> implementedContracts)
        {
            this.contract = new ContractDescription(name, ns);

            implementedContracts = new Dictionary<string, ContractDescription>() { { name, contract } };
            var sd = new ServiceDescription()
            {
                Name = this.name,
                Namespace = this.ns,
            };
            sd.Behaviors.Add(new ServiceBehaviorAttribute()
            {
                InstanceContextMode = InstanceContextMode.Single,
            });
            sd.Behaviors.Find<ServiceBehaviorAttribute>().SetWellKnownSingleton(new object());

            return sd;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
        }

        public EasyOperation WithOperation(string name)
        {
            var operation = new EasyOperation(this, name);
            this.contract.Operations.Add(operation.Operation);
            return operation;
        }

        public ContractDescription Contract
        {
            get { return contract; }
        }

        public EasyService At(int port)
        {
            return At("http://localhost:" + port);
        }

        public EasyService At(string localAddress)
        {
            //AddBaseAddress(new Uri(localAddress));
            var uri = new Uri(localAddress);
            this.AddServiceEndpoint(contract.Name, EasyBindings.GetDefaultBinding(uri.Scheme), uri);
            return this;
        }

        public EasyService Debuggable()
        {
            this.Description.Behaviors.Ensure<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
            return this;
        }
    }

    public class EasyOperation
    {
        readonly OperationDescription desc;
        readonly EasyService service;
        internal EasyOperation(EasyService service, string name)
        {
            this.service = service;
            desc = new OperationDescription(name, service.Contract);
            desc.Behaviors.Add(new OperationBehaviorAttribute()
            {
            });
            desc.Behaviors.Add(new DataContractSerializerOperationBehavior(desc)
            {

            });
        }

        public OperationDescription Operation
        {
            get { return desc; }
        }

        public EasyService Calls<T, TResult>(Func<T, TResult> method)
        {
            OperationBehaviorEvents.Install(desc).ApplyDispatchBehavior += (od, op) => 
            {
                op.Invoker = EasyMethodInvoker.Invoke(method);
            };
            desc.SyncMethod = method.Method;
            desc.Messages.Add(new MessageDescription(GetAction(desc.Name), MessageDirection.Input));
            desc.Messages[0].Body.WrapperName = desc.Name;
            desc.Messages[0].Body.WrapperNamespace = service.Contract.Namespace;
            foreach (var pi in method.Method.GetParameters())
            {
                desc.Messages[0].Body.Parts.Add(new MessagePartDescription(pi.Name, service.Contract.Namespace)
                {
                     Type = pi.ParameterType,
                });
            }
            desc.Messages.Add(new MessageDescription(GetAction(desc.Name), MessageDirection.Output));
            desc.Messages[1].Body.WrapperName = desc.Name + "Response";
            desc.Messages[1].Body.WrapperNamespace = service.Contract.Namespace;
            if (method.Method.ReturnType != typeof(void))
            {
                desc.Messages[1].Body.ReturnValue = new MessagePartDescription(desc.Name + "Result", service.Contract.Namespace)
                {
                     Type = method.Method.ReturnType,
                };
            }
            return this.service;
        }

        string GetAction(string methodName)
        {
            return service.Description.Namespace + service.Contract.Name + "/" + methodName;
        }
    }

    public static class EasyBindings
    {
        public static Binding GetDefaultBinding(string scheme)
        {
            switch (scheme)
            {
                case "http":
                    return new BasicHttpBinding();
                default:
                    throw new ArgumentException("unsupported scheme " + scheme);
            }
        }
    }

    public class EasyMethodInvoker : IOperationInvoker
    {
        object method;
        //public static IOperationInvoker Invoke(Func<object[], object> method)
        //{
        //    object[] parms = method.GetType().GetMethod("Invoke").GetParameters();
        //    object[] parms2 = method.Method.GetParameters();
        //    object o = parms[0];
        //    var t = method.GetInvocationList()[0].GetType();
        //    object[] parms3 = method.GetInvocationList()[0].Method.GetParameters();
        //    Console.WriteLine(o);
        //    return new EasyMethodInvoker()
        //    {
        //        method = method
        //    };
        //}

        public static IOperationInvoker Invoke<T, TResult>(Func<T, TResult> method)
        {
            return new EasyMethodInvoker()
            {
                method = method
            };
        }

        object[] IOperationInvoker.AllocateInputs()
        {
            return new object[((Delegate)method).Method.GetParameters().Length];
        }

        object IOperationInvoker.Invoke(object instance, object[] inputs, out object[] outputs)
        {
            outputs = new object[0];
            return ((Delegate)method).DynamicInvoke(inputs);
        }

        IAsyncResult IOperationInvoker.InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        object IOperationInvoker.InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        bool IOperationInvoker.IsSynchronous
        {
            get { return true; }
        }
    }

}