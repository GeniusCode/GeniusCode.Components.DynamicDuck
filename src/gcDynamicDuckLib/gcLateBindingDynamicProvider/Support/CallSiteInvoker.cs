using System;
using System.Runtime.CompilerServices;

namespace GeniusCode.Components.DynamicDuck.Providers.LateBinding
{
    public abstract class CallSiteInvoker
    {
        protected internal CallSite CallSite { get; set; }
        protected internal Delegate Delegate { get; set; }
    }
}
