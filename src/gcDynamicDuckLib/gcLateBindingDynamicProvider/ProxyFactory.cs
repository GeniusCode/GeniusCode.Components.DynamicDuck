using System;

namespace GeniusCode.Components.DynamicDuck.Providers
{
    public class DynamicProxyFactory
    {

#if !SILVERLIGHT

        public static T DuckCOMObject<T>(string identifier)
            where T : class
        {
            var t = Type.GetTypeFromProgID(identifier, false);
            dynamic speech = Activator.CreateInstance(t);
            T output = ProxyFactory.DuckInterface<T>(speech, new LateBindingInteractionProvider());
            return output;
        }
#endif
    }
}
