using System;
using System.Runtime.CompilerServices;

namespace GeniusCode.Components.DynamicDuck.Providers.LateBinding
{
    public class CallSiteGetPropertyInvoker : CallSiteInvoker
    {
        public object Invoke(object target)
        {
            return (Delegate as Func<CallSite, object, object>).Invoke(CallSite, target);
        }
    }

}


