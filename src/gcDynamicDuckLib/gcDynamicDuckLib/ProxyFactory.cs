using System.Collections.Generic;
using GeniusCode.Components.DynamicDuck.Providers;

namespace GeniusCode.Components.DynamicDuck
{
    public class ProxyFactory
    {



        public static T DuckInterface<T>(object input)
        where T : class
        {
            IDuckInteractionProvider provider = null;
            return DuckInterface<T>(input, provider);
        }

        public static T DuckInterface<T>(object input, IDuckInteractionProvider provider)
            where T : class
        {
            // don't set default values on duck
            return new ThunkFactory(provider).AsIf<T>(input, false);
        }

        public static T MockInterface<T>()
        where T : class
        {
            return MockInterface<T>(null, null);
        }

        public static T MockInterface<T>(IMockInteractionProvider provider, IDictionary<string, object> dictionary)
            where T : class
        {

            provider = provider ?? new DictionaryInteractionProvider();
            dictionary = dictionary ?? new Dictionary<string, object>();
            // set default values on mock
            return new ThunkFactory(provider).AsIf<T>(dictionary, true);
        }

    }
}
