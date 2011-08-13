namespace Microsoft.ServiceModel.Extras
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    public static partial class ServiceHostExtensionMethods
    {
        public static void AddMetadataMessageInspector(this ServiceHost host, IDispatchMessageInspector inspector)
        {
            host.ApplyDispatchBehavior((sd,sh) => 
            {
                foreach (var ed in host.GetMetadataEndpointDispatchers())
                {
                    ed.DispatchRuntime.MessageInspectors.Add(inspector);
                }
            });
        }

        public static IEnumerable<EndpointDispatcher> GetMetadataEndpointDispatchers(this ServiceHost host)
        {
            return host.GetEndpointDispatchersForContract(MetadataContract.Name, MetadataContract.Namespace).Union(
                   host.GetEndpointDispatchersForContract(HttpGetContract.Name, HttpGetContract.Namespace));
        }

        public static void InlineXsdsInWsdl(this ServiceHost host)
        {
            var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (smb == null)
            {
                throw new InvalidOperationException("host must have metadata enabled. enable metadata first.");
            }
            var wsdl = smb.MetadataExporter as WsdlExporter;
            if (wsdl == null)
            {
                throw new InvalidOperationException("ServiceMetadataBehavior.MetadataExporter is null or is not set to a WsdlExporter.");
            }
            host.AddMetadataMessageInspector(new EasyMetadataInterceptor()
            {
                Wsdl = doc => 
                {
                    var types = doc.Descendants(XName.Get("types", EasyMetadataInterceptor.WsdlNamespace)).FirstOrDefault();
                    if (types != null)
                    {
                        List<System.Xml.Schema.XmlSchema> xsdsToInline = new List<System.Xml.Schema.XmlSchema>();
                        List<XElement> importsToRemove = new List<XElement>();
                        foreach (var schema in types.Elements(XName.Get("schema", EasyMetadataInterceptor.XsdNamespace)))
                        {
                            foreach (var import in schema.Elements(XName.Get("import", EasyMetadataInterceptor.XsdNamespace)))
                            {
                                string tns = import.Attribute(XName.Get("namespace")).Value;
                                foreach (System.Xml.Schema.XmlSchema xsd in wsdl.GeneratedXmlSchemas.Schemas(tns))
                                {
                                    xsdsToInline.Add(xsd);
                                }
                                importsToRemove.Add(import);
                            }
                            foreach (var import in importsToRemove)
                            {
                                import.Remove();
                            }
                            importsToRemove.Clear();
                        }
                        foreach (var schema in types.Elements(XName.Get("schema", EasyMetadataInterceptor.XsdNamespace)))
                        {
                            if (schema.Elements().Count() == 0)
                            {
                                schema.Remove();
                            }
                        }
                        foreach (var xsd in xsdsToInline)
                        {
                            types.Add(SchemaAsXElement(xsd));
                        }
                    }
                    return doc;
                }
            });
        }

        static XElement SchemaAsXElement(System.Xml.Schema.XmlSchema xsd)
        {
            var enc = new UTF8Encoding(false);
            using (var w = new StringWriter())
            {
                xsd.Write(w);
                using (var r = new StringReader(w.ToString()))
                {
                    return XElement.Load(r);
                }
            }
        }

        static ContractDescription httpGetContract;
        static ContractDescription HttpGetContract
        {
            get
            {
                if (httpGetContract == null)
                {
                    httpGetContract = new ContractDescription("IHttpGetHelpPageAndMetadataContract", "http://schemas.microsoft.com/2006/04/http/metadata");
                }
                return httpGetContract;
            }
        }
        static ContractDescription metadataContract;
        static ContractDescription MetadataContract
        {
            get
            {
                if (metadataContract == null)
                {
                    metadataContract = ContractDescription.GetContract(typeof(IMetadataExchange));
                }
                return metadataContract;
            }
        }
    }

    public class EasyMetadataInterceptor : EasyMessageInterceptorBase
    {
        public const string WsdlNamespace = System.Web.Services.Description.ServiceDescription.Namespace;
        public const string XsdNamespace = System.Xml.Schema.XmlSchema.Namespace;

        public Func<XDocument, XDocument> Html { get; set; }
        public Func<XDocument, XDocument> Wsdl { get; set; }
        public Func<XDocument, XDocument> Xsd { get; set; }

        protected override XDocument ProcessReply(string contentType, XDocument body)
        {
            Func<XDocument, XDocument> func = null;

            if (body.Root.Name.LocalName == "HTML")
            {
                func = this.Html;
            }
            else if (body.Root.Name.LocalName == "schema" && body.Root.Name.Namespace == XsdNamespace)
            {
                func = this.Xsd;
            }
            else if (body.Root.Name.LocalName == "definitions" && body.Root.Name.Namespace == WsdlNamespace)
            {
                func = this.Wsdl;
            }
            return func != null ? func(body) : base.ProcessReply(contentType, body);
        }

        public static XElement CreateDocumentationAnnotation(string documentation, string lang)
        {
            return new XElement(XName.Get("annotation", EasyMetadataInterceptor.XsdNamespace),
                                new XElement(XName.Get("documentation", EasyMetadataInterceptor.XsdNamespace),
                                    new XAttribute(XNamespace.Xml.GetName("lang"), "en"),
                                    "this is documentation!!!"
                                    )
                                );
        }
    }

}