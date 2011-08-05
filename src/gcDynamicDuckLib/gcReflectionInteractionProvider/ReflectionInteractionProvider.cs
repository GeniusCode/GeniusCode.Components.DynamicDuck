using System;
using System.Linq;
using Fasterflect;

namespace GeniusCode.Components.DynamicDuck.Providers
{

    /// <summary>
    /// Provider that uses reflection to interaction with a dynamic object
    /// </summary>
    public class ReflectionInteractionProvider : DynamicProviderBase, IDuckInteractionProvider
    {

        protected override void RemoveHandler(object target, string name, Delegate handler, Type delegateType)
        {
            //not implemented:
            throw new NotImplementedException();
        }

        protected override void AddHandler(object target, string name, Delegate handler, Type delegateType)
        {
            //not implemented:
            throw new NotImplementedException();
        }

        protected override T PerformPropertyGet<T>(object target, string propertyName)
        {
            var _TypeToUse = target.GetType();

            var getter = _TypeToUse.DelegateForGetPropertyValue(propertyName, Flags.Public | Flags.Instance | Flags.NonPublic);
            return (T)getter.Invoke(target);
        }
        protected override void PerformPropertySet<T>(object target, string propertyName, T value)
        {
            var _TypeToUse = target.GetType();
            
            var setter = _TypeToUse.DelegateForSetPropertyValue(propertyName, Flags.Public | Flags.Instance | Flags.NonPublic);
            setter.Invoke(target, value);
        }

        protected override void InvokeVoidMethod(MethodCallSiteInfo info)
        {
            InvokeReturnMethod<object>(info);
        }

        protected override T InvokeReturnMethod<T>(MethodCallSiteInfo info)
        {
            var _TypeToUse = info.Target.GetType();


            var types = from t in info.Args
                        select t.ArguementType;

            var d = _TypeToUse.DelegateForCallMethod(info.MethodName, types.ToArray());


            var values = from t in info.Args
                         select t.ArguementValue;

            return (T)d.Invoke(info.Target, values.ToArray());
        }
    }
}
