using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GeniusCode.Components.Extensions;

namespace GeniusCode.Components.DynamicDuck.Providers
{

    /// <summary>
    /// Provider that treads a dynamic as an expando.  This provider supports mocking, but obviously not ducking
    /// </summary>
    public class DictionaryInteractionProvider : DynamicProviderBase, IMockInteractionProvider
    {
        protected override T PerformPropertyGet<T>(object target, string propertyName)
        {
            T result = default(T);
            target.TryAs<IDictionary<string, object>>(o =>
                    result = (T)o.CreateOrGetValue(propertyName, () => default(T)));

            return result;
        }

        protected override void PerformPropertySet<T>(object target, string propertyName, T value)
        {
            target.TryAs<IDictionary<string, object>>(o => o[propertyName] = value);
        }

        protected override T InvokeReturnMethod<T>(MethodCallSiteInfo info)
        {
            var args = info.Args.Select(a => a.ArguementValue).ToList();
            T result = default(T);

            info.Target.TryAs<IDictionary<string, object>>(d => d[info.MethodName]
                .TryAs<Delegate>(del => result = (T)del.DynamicInvoke(args)));

            return result;
        }

        protected override void InvokeVoidMethod(MethodCallSiteInfo info)
        {
            var args = info.Args.Select(a => a.ArguementValue).ToList();

            info.Target.TryAs<IDictionary<string, object>>(d => d[info.MethodName]
                .TryAs<Delegate>(del => del.DynamicInvoke(args)));
        }

        protected override bool Calculate_ShouldSetDefaultValue<T>(T value, object target, string propertyName)
        {
            bool result = true; //default is to set
            target.TryAs<IDictionary<string, object>>(d => result = !d.Keys.Contains(propertyName));
            return result;
        }

        protected override string GetQualifiedName(MemberInfo memberInfo)
        {
            string name = memberInfo.Name;

            memberInfo.TryAs<MethodInfo>(m =>
            {
                StringBuilder builder = new StringBuilder();
                m.GetParameters().ToList().ForEach(p => builder.Append("_" + p.ParameterType.Name));
                name = string.Format("{0}{1}", name, builder.ToString());
            });

            if (memberInfo.DeclaringType.IsInterface)
                return String.Format("{0}_{1}", memberInfo.DeclaringType.FullName, name);
            return memberInfo.Name;
        }


        protected override void AddHandler(object target, string name, Delegate handler, Type delegateType)
        {
            ////get event from dictionary:
            //target.TryAs<IDictionary<string,object>>(d=> d.CreateOrGetValue(name, Delegate. )
            //
            //EventHandler handler2;
            //EventHandler sampleEvent = this.SampleEvent;
            //do
            //{
            //    handler2 = sampleEvent;
            //    EventHandler handler3 = (EventHandler)Delegate.Remove(handler2, value);
            //    sampleEvent = Interlocked.CompareExchange<EventHandler>(ref this.SampleEvent, handler3, handler2);
            //}
            //while (sampleEvent != handler2);

        }

        protected override void RemoveHandler(object target, string name, Delegate handler, Type delegateType)
        {
            throw new NotImplementedException();
        }
    }
}
