using System;
using System.Runtime.CompilerServices;

namespace GeniusCode.Components.DynamicDuck.Providers.LateBinding
{
    public class CallSiteMethodInvoker : CallSiteInvoker
    {

        internal Func<CallSite, object, Delegate, object[], object> ParameterInvoker { get; set; }

        public object Invoke(object target, object[] args)
        {
            return ParameterInvoker.Invoke(CallSite, target, Delegate, args);
        }
    }
}
