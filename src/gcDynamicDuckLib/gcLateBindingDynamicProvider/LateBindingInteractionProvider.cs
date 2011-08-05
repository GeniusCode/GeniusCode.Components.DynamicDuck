using System;
using System.Collections.Generic;
using System.Linq;
using GeniusCode.Components.DynamicDuck.Providers.LateBinding;
using GeniusCode.Components.Extensions;

namespace GeniusCode.Components.DynamicDuck.Providers
{
    /// <summary>
    /// Provider that supports interacting with a dynamic object.  This provider does not currently support ducking,
    /// as the current infastructure requires strongly typing, while dynamic proxy only gives us the value type at runtime.
    /// 
    /// </summary>
    public class LateBindingInteractionProvider : DynamicProviderBase, IMockInteractionProvider, IDuckInteractionProvider
    {

        #region Assets
        private Dictionary<Tuple<Type, string>, CallSiteGetPropertyInvoker> getInvokers = new Dictionary<Tuple<Type, string>, CallSiteGetPropertyInvoker>();

        private Dictionary<Tuple<Type, string>, CallSiteSetPropertyInvoker> setInvokers = new Dictionary<Tuple<Type, string>, CallSiteSetPropertyInvoker>();

        private Dictionary<string, CallSiteMethodInvoker> methodReturnInvokers = new Dictionary<string, CallSiteMethodInvoker>();
        #endregion


        #region Overrides

        protected override void InvokeVoidMethod(MethodCallSiteInfo info)
        {
            throw new NotImplementedException();
        }
        protected override T InvokeReturnMethod<T>(MethodCallSiteInfo info)
        {
            var args = info.Args.Select(o => o.ArguementValue).ToArray();
            string key = string.Format("{0}__{1}", info.MethodName, info.Args.Count().ToString());
            CallSiteMethodInvoker invoker = methodReturnInvokers.CreateOrGetValue(key, () => LateBindingHelpers.CreateDynamicMethodInvoker(info.MethodName, args.Count()));

            return (T)invoker.Invoke(info.Target, args);
        }


        protected override void PerformPropertySet<T>(object target, string propertyName, T value)
        {

            Func<CallSiteSetPropertyInvoker> myFunc = () => LateBindingHelpers.CreateDynamicSetInvoker<T>(propertyName);

            var invoker = CachePoints ? setInvokers.CreateOrGetValue(new Tuple<Type, string>(target.GetType(), propertyName), myFunc)
                : myFunc();

            invoker.Invoke(target, value);
        }

        protected override T PerformPropertyGet<T>(object target, string propertyName)
        {
            Func<CallSiteGetPropertyInvoker> myFunc = () => LateBindingHelpers.CreateDynamicGetInvoker(propertyName);
            var invoker = CachePoints ? getInvokers.CreateOrGetValue(new Tuple<Type, string>(target.GetType(), propertyName), myFunc)
                : myFunc();

            return (T)invoker.Invoke(target);
        }



        #endregion



    }
}
