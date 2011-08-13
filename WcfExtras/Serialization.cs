namespace Microsoft.ServiceModel.Extras
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Collections.Generic;
    using System.Xml;

    public static partial class ServiceHostExtensionMethods
    {
        public static void AddDataContractSurrogate(this ServiceHostBase host, IDataContractSurrogate surrogate)
        {
            foreach (var endpoint in host.Description.Endpoints)
            {
                foreach (var operation in endpoint.Contract.Operations)
                {
                    var dc = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                    if (dc != null)
                    {
                        dc.DataContractSurrogate = surrogate;
                    }
                }
            }
            var exporter = host.GetXsdDataContractExporter();
            if (exporter != null)
            {
                exporter.Options.DataContractSurrogate = surrogate;
            }
        }

        public static XsdDataContractExporter GetXsdDataContractExporter(this ServiceHostBase host)
        {
            var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (smb != null) 
            {
                object o;
                if (!smb.MetadataExporter.State.TryGetValue(typeof(XsdDataContractExporter), out o))
                {
                    var wsdlexp = smb.MetadataExporter as WsdlExporter;
                    o = new XsdDataContractExporter(wsdlexp != null ? wsdlexp.GeneratedXmlSchemas : null);
                    smb.MetadataExporter.State.Add(typeof(XsdDataContractExporter), o);
                }
                var exp = (XsdDataContractExporter)o;
                if (exp.Options == null)
                {
                    exp.Options = new ExportOptions();
                }
                return exp;
            }
            return null;
        }

        public static void OnCreateSerializer(this ServiceHostBase host, CreateSerializer1 func)
        {
            host.ApplyDispatchBehavior(delegate
            {
                foreach (var operation in host.Description.AllOperations())
                {
                    DataContractSerializerOperationBehaviorExtended.Install(operation)
                        .CreateSerializer1 = func;
                }
            });
        }

        public static void OnCreateSerializer(this ServiceHostBase host, CreateSerializer2 func)
        {
            host.ApplyDispatchBehavior(delegate
            {
                foreach (var operation in host.Description.AllOperations())
                {
                    DataContractSerializerOperationBehaviorExtended.Install(operation)
                        .CreateSerializer2 = func;
                }
            });
        }

        public static void PreserveObjectReferences(this ServiceHostBase host)
        {
            host.ApplyDispatchBehavior(delegate
            {
                foreach (var operation in host.Description.AllOperations())
                {
                    DataContractSerializerOperationBehaviorExtended.Install(operation)
                        .PreserveObjectReferences = true;
                }
            });
        }

        public delegate XmlObjectSerializer CreateSerializer1(Type type, string name, string ns, IList<Type> knownTypes, DataContractSerializerOperationBehavior behavior);
        public delegate XmlObjectSerializer CreateSerializer2(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes, DataContractSerializerOperationBehavior behavior);

        public class DataContractSerializerOperationBehaviorExtended : DataContractSerializerOperationBehavior
        {
            public DataContractSerializerOperationBehaviorExtended(DataContractSerializerOperationBehavior inner,
                                                                    OperationDescription operation)
                : base(operation, inner.DataContractFormatAttribute)
            {
                this.DataContractSurrogate = inner.DataContractSurrogate;
                this.IgnoreExtensionDataObject = inner.IgnoreExtensionDataObject;
                this.MaxItemsInObjectGraph = inner.MaxItemsInObjectGraph;
            }

            public static DataContractSerializerOperationBehaviorExtended Install(OperationDescription operation)
            {
                var ext = operation.Behaviors.Find<DataContractSerializerOperationBehaviorExtended>();
                if (ext == null)
                {
                    var dc = operation.Behaviors.Remove<DataContractSerializerOperationBehavior>();
                    if (dc != null)
                    {
                        ext = new DataContractSerializerOperationBehaviorExtended(dc, operation);
                        operation.Behaviors.Add(ext);
                    }
                }
                return ext;
            }

            public bool PreserveObjectReferences { get; set; }

            public CreateSerializer1 CreateSerializer1 { get; set; }
            public CreateSerializer2 CreateSerializer2 { get; set; }

            public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
            {
                var result = CreateSerializer1(type, name, ns, knownTypes, this);
                return result ?? base.CreateSerializer(type, name, ns, knownTypes);
            }

            public override XmlObjectSerializer CreateSerializer(Type type, System.Xml.XmlDictionaryString name, System.Xml.XmlDictionaryString ns, System.Collections.Generic.IList<Type> knownTypes)
            {
                var result = CreateSerializer2(type, name, ns, knownTypes, this);
                return result ?? base.CreateSerializer(type, name, ns, knownTypes);
            }
        }
    }
}