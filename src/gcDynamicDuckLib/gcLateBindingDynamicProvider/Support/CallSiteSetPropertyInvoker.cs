using System;
using System.Runtime.CompilerServices;

namespace GeniusCode.Components.DynamicDuck.Providers.LateBinding
{
    public class CallSiteSetPropertyInvoker : CallSiteInvoker
    {
        public void Invoke<TValue>(object target, TValue value)
        {
            var del = Delegate as Func<CallSite, object, TValue, object>;
            del(CallSite, target, value); //why is this null for my late binding provider after I set default values?
        }
    }
}
