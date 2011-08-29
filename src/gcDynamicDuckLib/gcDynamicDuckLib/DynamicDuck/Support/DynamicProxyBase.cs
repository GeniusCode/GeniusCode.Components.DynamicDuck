using System;
using System.ComponentModel;

namespace GeniusCode.Components.DynamicDuck.Support
{



    /// <summary>
    /// Base class for dynamic objects.
    /// </summary>
    public abstract class DynamicProxyBase : IDynamicProxy, INotifyPropertyChanged
    {
        public object Target { get; private set; }
        public IDynamicInteractionProvider Provider { get; private set; }


        /// <summary>
        /// Creates a new dynamic object wrapping the specified <paramref name="target">target object</paramref>.
        /// </summary>
        /// <param name="target">Wrapped target object.</param>
        /// <param name="provider"></param>
        protected DynamicProxyBase(object target, IDynamicInteractionProvider provider)
        {
            Target = target;
            Provider = provider;
        }


        /// <summary>
        /// Determines whether the wrapped object refers to the same object as the object wrapped by the passed in dynamic object.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>true if the wrapped object refers to the same object as the object wrapped by the passed in dynamic object; false otherwise.</returns>
        /// <remarks>Equality between a dynamic wrapped object and a non-wrapped object object always returns false as we can't guarantee commutativity.</remarks>
        public override bool Equals(object obj)
        {
            var dynamic = obj as DynamicProxyBase;

            return dynamic != null && ReferenceEquals(dynamic.Target, Target);
        }

        /// <summary>
        /// Returns the hash code of the wrapped object.
        /// </summary>
        /// <returns>Hash code of the wrapped object.</returns>
        public override int GetHashCode()
        {
            return Target.GetHashCode();
        }

        protected IArgInfo GetArgInfo<T>(string argName, T value)
        {
            return new ArgInfo<T>(argName, value);
        }

        /// <summary>
        /// Returns the string representation of the wrapped object.
        /// </summary>
        /// <returns>String representation of the wrapped object.</returns>
        public override string ToString()
        {
            return Target.ToString();
        }

        /// <summary>
        /// Will create a default value for a property that has not been set on the expando
        /// (This is useful for silverlight.)
        /// </summary>
        protected bool AutoCreatePropertyOnGet { get; set; }

        protected T GetProperty<T>(string member)
        {
            return Provider.PerformPropertyGet<T>(Target, member);
        }

        protected void SetProperty<T>(string member, T val)
        {
            Provider.PerformPropertySet(val, Target, member);
        }

        protected void SetDefaultValueWithoutFunc<T>(string member)
        {
            SetDefaultValue(member, default(T));
        }
        protected void SetDefaultValueUsingFunc<T>(string member, Func<string, object> valueGetter)
        {
            Provider.SetDefaultValue((T)valueGetter(member), Target, member);
        }

        private void SetDefaultValue<T>(string member, T value)
        {
            Provider.SetDefaultValue(value, Target, member);
        }

        internal void SetDefaultValues(Func<string, object> valueGetter)
        {
            OnSetDefaultValues(valueGetter);
        }


        protected virtual void OnSetDefaultValues(Func<string, object> valueGetter) { }


        protected void InvokeVoidMethod(string methodName, params IArgInfo[] args)
        {
            args = args ?? new IArgInfo[] { };
            Provider.InvokeVoidMethod(new MethodCallSiteInfo { Target = Target, Args = args, MethodName = methodName });
        }

        protected T InvokeReturnMethod<T>(string methodName, params IArgInfo[] args)
        {
            args = args ?? new IArgInfo[] { };
            return Provider.InvokeReturnMethod<T>(new MethodCallSiteInfo { Target = Target, Args = args, MethodName = methodName });
        }

        protected void AddHandler<T>(string name, Delegate handler)
        {
            Provider.AddHandler(Target, name, handler, typeof(T));
        }

        protected void RemoveHandler<T>(string name, Delegate handler)
        {
            Provider.RemoveHandler(Target, name, handler, typeof(T));
        }


        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (Target is INotifyPropertyChanged)
                    (Target as INotifyPropertyChanged).PropertyChanged += value;
            }

            remove
            {
                if (Target is INotifyPropertyChanged)
                    (Target as INotifyPropertyChanged).PropertyChanged -= value;
            }
        }
    }
}
