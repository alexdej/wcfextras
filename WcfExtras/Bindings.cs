namespace Microsoft.ServiceModel.Extras
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;
    using System.Text;

    public static class BindingExtensionMethods
    {
        public static ICommonBindingProperties CommonProperties(this Binding binding)
        {
            return CommonBindingProperties.Create(binding);
        }

        public static ICommonHttpBindingProperties CommonHttpProperties(this Binding binding)
        {
            return CommonHttpBindingProperties.Create(binding);
        }

        public static ICommonConnectionBindingProperties CommonConnectionProperties(this Binding binding)
        {
            return CommonConnectionBindingProperties.Create(binding);
        }

        public static ICommonBindingProperties CommonProperties(this IEnumerable<Binding> bindings)
        {
            return CommonBindingProperties.Create(bindings);
        }

        public static ICommonHttpBindingProperties CommonHttpProperties(this IEnumerable<Binding> bindings)
        {
            return CommonHttpBindingProperties.Create(bindings);
        }

        public static ICommonConnectionBindingProperties CommonConnectionProperties(this IEnumerable<Binding> bindings)
        {
            return CommonConnectionBindingProperties.Create(bindings);
        }

    }

    public interface ICommonBindingProperties
    {
        EnvelopeVersion EnvelopeVersion { get; }
        HostNameComparisonMode HostNameComparisonMode { get; set; }
        long MaxBufferPoolSize { get; set; }
        int MaxBufferSize { get; set; }
        long MaxReceivedMessageSize { get; set; }
        XmlDictionaryReaderQuotas ReaderQuotas { get; set; }
        bool TransactionFlow { get; set; }
    }

    public interface ICommonHttpBindingProperties : ICommonBindingProperties
    {
        bool AllowCookies { get; set; }
        bool BypassProxyOnLocal { get; set; }
        WSMessageEncoding MessageEncoding { get; set; }
        Uri ProxyAddress { get; set; }
        Encoding TextEncoding { get; set; }
        bool UseDefaultWebProxy { get; set; }
    }

    public interface ICommonConnectionBindingProperties : ICommonBindingProperties
    {
        int MaxConnections { get; set; }
    }

    abstract class CommonBindingProperties : ICommonBindingProperties
    {
        public abstract EnvelopeVersion EnvelopeVersion { get; }
        public abstract HostNameComparisonMode HostNameComparisonMode { get; set; }
        public abstract long MaxBufferPoolSize { get; set; }
        public abstract int MaxBufferSize { get; set; }
        public abstract long MaxReceivedMessageSize { get; set; }
        public abstract XmlDictionaryReaderQuotas ReaderQuotas { get; set; }
        public abstract bool TransactionFlow { get; set; }

        public static ICommonBindingProperties Create(Binding binding)
        {
            // TODO: fill out the rest of the binding types
            // TODO: CustomBinding
            if (binding is ICommonBindingProperties)
            {
                return (ICommonBindingProperties)binding;
            }
            else if (binding is BasicHttpBinding)
            {
                return new BasicHttpBindingCommonProperties((BasicHttpBinding)binding);
            }
            else if (binding is NetTcpBinding)
            {
                return new NetTcpBindingCommonProperties((NetTcpBinding)binding);
            }
            else if (binding is NetNamedPipeBinding)
            {
                return new NetNamedPipeBindingCommonProperties((NetNamedPipeBinding)binding);
            }
            else if (binding is WSHttpBindingBase)
            {
                return new WSHttpBindingBaseCommonProperties((WSHttpBindingBase)binding);
            }
            else
            {
                // CONSIDER: throw an exception?
                return new EmptyCommonBindingProperties();
            }
        }

        public static ICommonBindingProperties Create(IEnumerable<Binding> bindings)
        {
            return new AggregateCommonProperties(bindings.Select(b => Create(b)));
        }
    }
    abstract class CommonBindingProperties<TBinding> : CommonBindingProperties where TBinding : Binding
    {
        protected CommonBindingProperties(TBinding binding)
        {
            this.Binding = binding;
        }

        protected TBinding Binding { get; set; }
    }

    abstract class CommonHttpBindingProperties : CommonBindingProperties, ICommonHttpBindingProperties
    {
        public abstract bool AllowCookies { get; set; }
        public abstract bool BypassProxyOnLocal { get; set; }
        public abstract WSMessageEncoding MessageEncoding { get; set; }
        public abstract Uri ProxyAddress { get; set; }
        public abstract Encoding TextEncoding { get; set; }
        public abstract bool UseDefaultWebProxy { get; set; }

        public static new ICommonHttpBindingProperties Create(Binding binding)
        {
            var properties = CommonBindingProperties.Create(binding) as ICommonHttpBindingProperties;
            if (properties == null)
            {
                // CONSIDER: throw an exception?
                return new EmptyCommonBindingProperties();
            }
            return properties;
        }

        public static new ICommonHttpBindingProperties Create(IEnumerable<Binding> bindings)
        {
            return (ICommonHttpBindingProperties)CommonBindingProperties.Create(bindings);
        }
    }

    abstract class CommonHttpBindingProperties<TBinding> : CommonHttpBindingProperties where TBinding : Binding
    {
        protected CommonHttpBindingProperties(TBinding binding)
        {
            this.Binding = binding;
        }
        protected TBinding Binding { get; set; }
    }

    abstract class CommonConnectionBindingProperties : CommonBindingProperties, ICommonConnectionBindingProperties
    {
        public abstract int MaxConnections { get; set; }

        public static new ICommonConnectionBindingProperties Create(Binding binding)
        {
            var properties = CommonBindingProperties.Create(binding) as ICommonConnectionBindingProperties;
            if (properties == null)
            {
                // CONSIDER: throw an exception?
                return new EmptyCommonBindingProperties();
            }
            return properties;
        }

        public static new ICommonConnectionBindingProperties Create(IEnumerable<Binding> bindings)
        {
            return (ICommonConnectionBindingProperties)CommonBindingProperties.Create(bindings);
        }
    }

    abstract class CommonConnectionBindingProperties<TBinding> : CommonConnectionBindingProperties where TBinding : Binding
    {
        protected CommonConnectionBindingProperties(TBinding binding)
        {
            this.Binding = binding;
        }
        protected TBinding Binding { get; set; }
    }

    class BasicHttpBindingCommonProperties : CommonHttpBindingProperties<BasicHttpBinding>
    {
        public BasicHttpBindingCommonProperties(BasicHttpBinding binding) : base(binding) { }

        public override EnvelopeVersion EnvelopeVersion
        {
            get { return Binding.EnvelopeVersion; }
        }

        public override HostNameComparisonMode HostNameComparisonMode
        {
            get { return Binding.HostNameComparisonMode; }
            set { Binding.HostNameComparisonMode = value; }
        }

        public override long MaxBufferPoolSize
        {
            get { return Binding.MaxBufferPoolSize; }
            set { Binding.MaxBufferPoolSize = value; }
        }

        public override int MaxBufferSize
        {
            get { return Binding.MaxBufferSize; }
            set { Binding.MaxBufferSize = value; }
        }

        public override long MaxReceivedMessageSize
        {
            get { return Binding.MaxReceivedMessageSize; }
            set { Binding.MaxReceivedMessageSize = value; }
        }

        public override XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return Binding.ReaderQuotas; }
            set { Binding.ReaderQuotas = value; }
        }

        public override bool TransactionFlow
        {
            get { return false; }
            set { }
        }

        public override bool AllowCookies
        {
            get { return Binding.AllowCookies; }
            set { Binding.AllowCookies = value; }
        }

        public override bool BypassProxyOnLocal
        {
            get { return Binding.BypassProxyOnLocal; }
            set { Binding.BypassProxyOnLocal = value; }
        }

        public override WSMessageEncoding MessageEncoding
        {
            get { return Binding.MessageEncoding; }
            set { Binding.MessageEncoding = value; }
        }

        public override Uri ProxyAddress
        {
            get { return Binding.ProxyAddress; }
            set { Binding.ProxyAddress = value; }
        }

        public override Encoding TextEncoding
        {
            get { return Binding.TextEncoding; }
            set { Binding.TextEncoding = value; }
        }

        public override bool UseDefaultWebProxy
        {
            get { return Binding.UseDefaultWebProxy; }
            set { Binding.UseDefaultWebProxy = value; }
        }
    }

    class NetTcpBindingCommonProperties : CommonConnectionBindingProperties<NetTcpBinding>
    {
        public NetTcpBindingCommonProperties(NetTcpBinding binding) : base(binding) { }

        public override EnvelopeVersion EnvelopeVersion
        {
            get { return Binding.EnvelopeVersion; }
        }

        public override HostNameComparisonMode HostNameComparisonMode
        {
            get { return Binding.HostNameComparisonMode; }
            set { Binding.HostNameComparisonMode = value; }
        }

        public override long MaxBufferPoolSize
        {
            get { return Binding.MaxBufferPoolSize; }
            set { Binding.MaxBufferPoolSize = value; }
        }

        public override int MaxBufferSize
        {
            get { return Binding.MaxBufferSize; }
            set { Binding.MaxBufferSize = value; }
        }

        public override long MaxReceivedMessageSize
        {
            get { return Binding.MaxReceivedMessageSize; }
            set { Binding.MaxReceivedMessageSize = value; }
        }

        public override XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return Binding.ReaderQuotas; }
            set { Binding.ReaderQuotas = value; }
        }

        public override bool TransactionFlow
        {
            get { return Binding.TransactionFlow; }
            set { Binding.TransactionFlow = value;  }
        }

        public override int MaxConnections
        {
            get { return Binding.MaxConnections; }
            set { Binding.MaxConnections = value; }
        }
    }

    class NetNamedPipeBindingCommonProperties : CommonConnectionBindingProperties<NetNamedPipeBinding>
    {
        public NetNamedPipeBindingCommonProperties(NetNamedPipeBinding binding) : base(binding) { }

        public override EnvelopeVersion EnvelopeVersion
        {
            get { return Binding.EnvelopeVersion; }
        }

        public override HostNameComparisonMode HostNameComparisonMode
        {
            get { return Binding.HostNameComparisonMode; }
            set { Binding.HostNameComparisonMode = value; }
        }

        public override long MaxBufferPoolSize
        {
            get { return Binding.MaxBufferPoolSize; }
            set { Binding.MaxBufferPoolSize = value; }
        }

        public override int MaxBufferSize
        {
            get { return Binding.MaxBufferSize; }
            set { Binding.MaxBufferSize = value; }
        }

        public override long MaxReceivedMessageSize
        {
            get { return Binding.MaxReceivedMessageSize; }
            set { Binding.MaxReceivedMessageSize = value; }
        }

        public override XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return Binding.ReaderQuotas; }
            set { Binding.ReaderQuotas = value; }
        }

        public override bool TransactionFlow
        {
            get { return Binding.TransactionFlow; }
            set { Binding.TransactionFlow = value; }
        }

        public override int MaxConnections
        {
            get { return Binding.MaxConnections; }
            set { Binding.MaxConnections = value; }
        }
    }

    class WSHttpBindingBaseCommonProperties : CommonHttpBindingProperties<WSHttpBindingBase>
    {
        public WSHttpBindingBaseCommonProperties(WSHttpBindingBase binding) : base(binding) { }

        public override EnvelopeVersion EnvelopeVersion
        {
            get { return Binding.EnvelopeVersion; }
        }

        public override HostNameComparisonMode HostNameComparisonMode
        {
            get { return Binding.HostNameComparisonMode; }
            set { Binding.HostNameComparisonMode = value; }
        }

        public override long MaxBufferPoolSize
        {
            get { return Binding.MaxBufferPoolSize; }
            set { Binding.MaxBufferPoolSize = value; }
        }

        public override int MaxBufferSize
        {
            // CONSIDER, alexdej: should this throw maybe? or just no-op
            get { return 0; }
            set { }
        }

        public override long MaxReceivedMessageSize
        {
            get { return Binding.MaxReceivedMessageSize; }
            set { Binding.MaxReceivedMessageSize = value; }
        }

        public override XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return Binding.ReaderQuotas; }
            set { Binding.ReaderQuotas = value; }
        }

        public override bool TransactionFlow
        {
            get { return Binding.TransactionFlow; }
            set { Binding.TransactionFlow = value; }
        }

        public override bool AllowCookies
        {
            // TODO, alexdej: add the derived class
            get { return false; }
            set { }
        }

        public override bool BypassProxyOnLocal
        {
            get { return Binding.BypassProxyOnLocal; }
            set { Binding.BypassProxyOnLocal = value; }
        }

        public override WSMessageEncoding MessageEncoding
        {
            get { return Binding.MessageEncoding; }
            set { Binding.MessageEncoding = value; }
        }

        public override Uri ProxyAddress
        {
            get { return Binding.ProxyAddress; }
            set { Binding.ProxyAddress = value; }
        }

        public override Encoding TextEncoding
        {
            get { return Binding.TextEncoding; }
            set { Binding.TextEncoding = value; }
        }

        public override bool UseDefaultWebProxy
        {
            get { return Binding.UseDefaultWebProxy; }
            set { Binding.UseDefaultWebProxy = value; }
        }
    }

    class AggregateCommonProperties : CommonBindingProperties, ICommonHttpBindingProperties, ICommonConnectionBindingProperties
    {
        public AggregateCommonProperties(IEnumerable<ICommonBindingProperties> bindings)
        {
            this.Bindings = bindings;
        }
        public IEnumerable<ICommonBindingProperties> Bindings;

        public override EnvelopeVersion EnvelopeVersion
        {
            get { throw new InvalidOperationException(); }
        }

        public override HostNameComparisonMode HostNameComparisonMode
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings)
                {
                    binding.HostNameComparisonMode = value;
                }
            }
        }

        public override long MaxBufferPoolSize
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings)
                {
                    binding.MaxBufferPoolSize = value;
                }
            }
        }

        public override int MaxBufferSize
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings)
                {
                    binding.MaxBufferSize = value;
                }
            }
        }

        public override long MaxReceivedMessageSize
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings)
                {
                    binding.MaxReceivedMessageSize = value;
                }
            }
        }

        public override XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings)
                {
                    binding.ReaderQuotas = value;
                }
            }
        }

        public override bool TransactionFlow
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings)
                {
                    binding.TransactionFlow = value;
                }
            }
        }

        #region ICommonHttpBindingProperties Members

        public bool AllowCookies
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings.OfType<ICommonHttpBindingProperties>())
                {
                    binding.AllowCookies = value;
                }
            }
        }

        public bool BypassProxyOnLocal
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings.OfType<ICommonHttpBindingProperties>())
                {
                    binding.BypassProxyOnLocal = value;
                }
            }
        }

        public WSMessageEncoding MessageEncoding
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings.OfType<ICommonHttpBindingProperties>())
                {
                    binding.MessageEncoding = value;
                }
            }
        }

        public Uri ProxyAddress
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings.OfType<ICommonHttpBindingProperties>())
                {
                    binding.ProxyAddress = value;
                }
            }
        }

        public Encoding TextEncoding
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings.OfType<ICommonHttpBindingProperties>())
                {
                    binding.TextEncoding = value;
                }
            }
        }

        public bool UseDefaultWebProxy
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings.OfType<ICommonHttpBindingProperties>())
                {
                    binding.UseDefaultWebProxy = value;
                }
            }
        }

        #endregion

        #region ICommonConnectionBindingProperties Members

        public int MaxConnections
        {
            get { throw new InvalidOperationException(); }
            set
            {
                foreach (var binding in Bindings.OfType<ICommonConnectionBindingProperties>())
                {
                    binding.MaxConnections = value;
                }
            }
        }

        #endregion
    }

    class EmptyCommonBindingProperties : CommonBindingProperties, ICommonHttpBindingProperties, ICommonConnectionBindingProperties
    {
        public override EnvelopeVersion EnvelopeVersion
        {
            get { return null; }
        }

        public override HostNameComparisonMode HostNameComparisonMode
        {
            get { return default(HostNameComparisonMode); }
            set { }
        }

        public override long MaxBufferPoolSize
        {
            get { return default(long); }
            set { }
        }

        public override int MaxBufferSize
        {
            get { return default(int); }
            set { }
        }

        public override long MaxReceivedMessageSize
        {
            get { return default(long); }
            set { }
        }

        public override XmlDictionaryReaderQuotas ReaderQuotas
        {
            get { return null; }
            set { }
        }

        public override bool TransactionFlow
        {
            get { return default(bool); }
            set { }
        }

        #region ICommonHttpBindingProperties Members

        public bool AllowCookies
        {
            get { return default(bool); }
            set { }
        }

        public bool BypassProxyOnLocal
        {
            get { return default(bool); }
            set { }
        }

        public WSMessageEncoding MessageEncoding
        {
            get { return default(WSMessageEncoding); }
            set { }
        }

        public Uri ProxyAddress
        {
            get { return null; }
            set { }
        }

        public Encoding TextEncoding
        {
            get { return null; }
            set { }
        }

        public bool UseDefaultWebProxy
        {
            get { return default(bool); }
            set { }
        }

        #endregion

        #region ICommonConnectionBindingProperties Members

        public int MaxConnections
        {
            get { return default(int); }
            set { }
        }

        #endregion
    }
}