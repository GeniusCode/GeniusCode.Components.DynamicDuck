using System;
using System.Linq;
using System.Reflection;

namespace GeniusCode.Components.DynamicDuck.Providers
{

    /// <summary>
    /// Provider that uses reflection to interaction with a dynamic object
    /// </summary>
    public class ReflectionInteractionProvider : DynamicProviderBase, IDuckInteractionProvider
    {
        private const BindingFlags _BindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

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



        private static PropertyInfo GetPropertyInfo(string propertyName, object target)
        {
            var type = target.GetType();
            return type.GetProperty(propertyName, _BindingFlags);
        }

        protected override T PerformPropertyGet<T>(object target, string propertyName)
        {
            var propertyInfo = GetPropertyInfo(propertyName, target);
            var value = (T)propertyInfo.GetValue(target, null);
            return value;
        }
        protected override void PerformPropertySet<T>(object target, string propertyName, T value)
        {
            var propertyInfo = GetPropertyInfo(propertyName, target);
            propertyInfo.SetValue(target, value, null);
        }

        protected override void InvokeVoidMethod(MethodCallSiteInfo info)
        {
            InvokeReturnMethod<object>(info);
        }

        protected override T InvokeReturnMethod<T>(MethodCallSiteInfo info)
        {
            var typeToUse = info.Target.GetType();


            var types = (from t in info.Args
                         select t.ArguementType).ToArray();

            var mi = typeToUse.GetMethod(info.MethodName, _BindingFlags, null, CallingConventions.HasThis, types, null);

            var values = (from t in info.Args
                          select t.ArguementValue).ToArray();

            return (T)mi.Invoke(info.Target, values);
        }
    }
}
