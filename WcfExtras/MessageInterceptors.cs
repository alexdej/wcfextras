namespace Microsoft.ServiceModel.Extras
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    public class EasyMessageInterceptor : EasyMessageInterceptorBase
    {
        public Func<string, XDocument, XDocument> Request;
        public Func<string, XDocument, XDocument> Reply;

        protected override XDocument ProcessRequest(string contentType, XDocument body)
        {
            var func = Reply;
            return func != null ? func(contentType, body) : base.ProcessRequest(contentType, body);
        }

        protected override XDocument ProcessReply(string contentType, XDocument body)
        {
            var func = Reply;
            return func != null ? func(contentType, body) : base.ProcessReply(contentType, body);
        }
    }
    
    public class EasyMessageInterceptorBase : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            request = new InterceptingMessage(request)
            {
                Process = this.ProcessRequest,
            };
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            reply = new InterceptingMessage(reply)
            {
                Process = this.ProcessReply,
            };
        }

        protected virtual XDocument ProcessRequest(string contentType, XDocument body)
        {
            return body;
        }

        protected virtual XDocument ProcessReply(string contentType, XDocument body)
        {
            return body;
        }

        class InterceptingMessage : Message
        {
            readonly Message inner;
            public InterceptingMessage(Message inner)
            {
                this.inner = inner;
            }

            public Func<string, XDocument, XDocument> Process { get; set; }

            public override MessageHeaders Headers
            {
                get { return this.inner.Headers; }
            }

            protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer)
            {
                XDocument body = GetMessageBodyAsElement(this.inner);
                OnProcess("unknown", body).WriteTo(writer);
            }

            protected virtual XDocument OnProcess(string contentType, XDocument body)
            {
                return this.Process != null ? this.Process(contentType, body) : body;
            }

            static XDocument GetMessageBodyAsElement(Message m)
            {
                var enc = new UTF8Encoding(false);
                using (var buf = new MemoryStream())
                {
                    using (var w = XmlDictionaryWriter.CreateTextWriter(buf, enc, false))
                    {
                        m.WriteBodyContents(w);
                    }

                    buf.Position = 0;

                    using (var r = new StreamReader(buf, enc))
                    {
                        return XDocument.Load(r);
                    }
                }
            }

            public override MessageProperties Properties
            {
                get { return this.inner.Properties; }
            }

            public override MessageVersion Version
            {
                get { return this.inner.Version; }
            }
        }
    }
}