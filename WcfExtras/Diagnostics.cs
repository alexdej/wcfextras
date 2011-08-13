namespace Microsoft.ServiceModel.Extras
{
    using System;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel;
    using System.Collections.Generic;
    using System.ServiceModel.Description;
    using System.ServiceModel.Channels;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;

    public static partial class ServiceHostExtensionMethods
    {
        public static void TraceTo(this ServiceHostBase host, TraceListener listener, SourceLevels level, params WcfTraceSource[] sources)
        {
            // TODO: probably should restrict this to a particular host given that we're hanging this method there

            for (int i = 0; i < sources.Length; i++)
            {
                var enabled = OnCreateTraceSource(sources[i], source =>
                {
                    source.Listeners.Add(listener);
                    source.Switch.Level = level;
                });
                if (!enabled)
                    throw new InvalidOperationException("something went wrong");
            }

        }

        // this method uses major private reflection hackery to insert a callback into the tracing system's init process.
        // this is necessary because DiagnosticUtility auto-disables tracing if the TraceSource does not have any Listeners
        // or if the TraceSwitch level is set to none. (this information is loaded from config).
        //
        // the hackery is to re-initialize the tracing step by step with an inserted callback in the middle
        // assuming the callback adds Listeners *and* ups the TraceSwitch.Level, then Tracing will be re-enabled
        static bool OnCreateTraceSource(WcfTraceSource wcfTraceSource, Action<TraceSource> callback)
        {
            var utilityType = InternalType.GetType(diagnosticUtilities[(int)wcfTraceSource]);
            var diagnosticTraceProperty = utilityType.GetProperty("DiagnosticTrace", BindingFlags.Static);

            // if we create a new one we'll need to add some cleanup hooks later on
            bool createdNewDiagnosticTrace = false;

            // ensure that DiagnosticUtility.DiagnosticTrace != null
            var trace = diagnosticTraceProperty.Get(null);
            if (trace == null)
            {
                // force re-initialization of the DiagnosticUtility, so that we can see the DiagnosticTrace object before it gets nulled out
                utilityType.GetMethod("InitDiagnosticTraceImpl", BindingFlags.Static)
                    .Invoke(null, 0/*TraceSourceKind.DiagnosticTraceSource*/, "System.ServiceModel");
                trace = diagnosticTraceProperty.Get(null);

                createdNewDiagnosticTrace = true;
            }

            // then get the TraceSource..
            var traceType = new InternalType(trace.GetType());
            var traceSource = traceType.GetProperty("TraceSource").Get<TraceSource>(trace);

            // .. and call the callback
            if (callback != null)
            {
                callback(traceSource);
            }

            // add cleanup hooks if necessary
            if (createdNewDiagnosticTrace)
            {
                traceType.GetMethod("UnsafeAddDomainEventHandlersForCleanup").Invoke(trace);
            }

            // tell DiagnosticUtiltiy to update itself based on the changed DiagnosticTrace object
            utilityType.GetMethod("UpdateLevel", BindingFlags.Static).Invoke(null);

            // check whether tracing got enabled and return that
            return utilityType.GetProperty("TracingEnabled", BindingFlags.Static).Get<bool>(trace);
        }

        static readonly string[] diagnosticUtilities = new string[]
        {
            "System.ServiceModel.DiagnosticUtility, System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
            null,
            null,
        };
    }

    public enum WcfTraceSource
    {
        ServiceModel,
        IdentityModel,
        Serialization,
    }

    class InternalType
    {
        Type type;

        public InternalType(Type type)
        {
            this.type = type;
        }

        public static InternalType GetType(string assemblyQualifiedTypeName)
        {
            return new InternalType(Type.GetType(assemblyQualifiedTypeName, true));
        }

        public InternalProperty GetProperty(string name)
        {
            return GetProperty(name, BindingFlags.Instance);
        }

        public InternalProperty GetProperty(string name, BindingFlags bindingFlags)
        {
            return new InternalProperty(this.type.GetProperty(name, bindingFlags | BindingFlags.NonPublic));
        }

        public InternalMethod GetMethod(string name)
        {
            return GetMethod(name, BindingFlags.Instance);
        }

        public InternalMethod GetMethod(string name, BindingFlags bindingFlags)
        {
            return new InternalMethod(this.type.GetMethod(name, bindingFlags | BindingFlags.NonPublic));
        }
    }

    class InternalProperty
    {
        PropertyInfo property;
        Func<object, object> getter;
        Action<object, object> setter;

        public InternalProperty(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            
            this.property = property;
        }

        public object Get(object o)
        {
            if (getter == null)
            {
                this.getter = ExpressionHelper.BuildPropertyGetter<object>(property);
            }
            return this.getter(o);
        }

        public T Get<T>(object o)
        {
            return ExpressionHelper.BuildPropertyGetter<T>(property)(o);
        }

        public void Set(object o, object value)
        {
            if (setter == null)
            {
                setter = ExpressionHelper.BuildPropertySetter(property);
            }
            setter(o, value);
        }
    }

    class InternalMethod
    {
        MethodInfo method;

        public InternalMethod(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            this.method = method;
        }

        public object Invoke(object o, params object[] args)
        {
            return this.method.Invoke(o, args);
        }
    }

    static class ExpressionHelper
    {
        public static Func<object, T> BuildPropertyGetter<T>(PropertyInfo property)
        {
            var arg = Expression.Parameter(typeof(object), "arg");

            return (Func<object, T>)Expression.Lambda(
                Expression.Convert(
                    Expression.Property(
                        Expression.Convert(arg, property.DeclaringType),
                        property
                    ),
                    typeof(T)
                ),
                arg
            ).Compile();
        }

        public static Action<object, object> BuildPropertySetter(PropertyInfo property)
        {
            throw new NotImplementedException();
        }
    }
}