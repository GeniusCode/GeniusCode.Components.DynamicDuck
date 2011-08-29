using System.Collections.Generic;

namespace GeniusCode.Components.DynamicDuck.Support
{
    internal class NestedDictionary<TOuterKey, TInnerKey, TValue> : Dictionary<TOuterKey, Dictionary<TInnerKey, TValue>>
    { }
}
