using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeniusCode.Components.DynamicDuck
{
    internal class NestedDictionary<TOuterKey, TInnerKey, TValue> : Dictionary<TOuterKey, Dictionary<TInnerKey, TValue>>
    { }
}
