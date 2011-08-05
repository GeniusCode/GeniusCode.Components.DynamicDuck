using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;


namespace GeniusCode.Components.DynamicDuck.Providers.LateBinding
{

    /// <remarks>http://stackoverflow.com/questions/1926776/getting-a-value-from-a-dynamic-object-dynamically</remarks>
    public class LateBindingHelpers
    {

        #region Assets

        private static CSharpArgumentInfo[] GetEmptyCSharpArgs(int count)
        {
            var args = new List<CSharpArgumentInfo>(count);

            for (int i = 0; i < count; i++)
            {
                var ai = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
                args.Add(ai);
            }

            return args.ToArray();
        }
        #endregion


        public static CallSiteMethodInvoker CreateDynamicMethodInvoker(string methodName, int argsCount)
        {
            var tuple = CreateDynamicInvokerBinderAndInvokerFunc(methodName, argsCount);
            var invoker = new CallSiteMethodInvoker() { CallSite = tuple.Item1, Delegate = tuple.Item2, ParameterInvoker = tuple.Item3 };
            return invoker;
        }


        #region Public Invoker Factory Methods

        public static CallSiteGetPropertyInvoker CreateDynamicGetInvoker(string propertyName)
        {
            var site = CreatePropertyGetCallSiteBinder(propertyName);
            var i = new CallSiteGetPropertyInvoker() { CallSite = site, Delegate = site.Target };
            return i;
        }

        public static CallSiteSetPropertyInvoker CreateDynamicSetInvoker<TValue>(string propertyName)
        {
            var site = CreatePropertySetCallSiteBinder<TValue>(propertyName);
            return new CallSiteSetPropertyInvoker() { Delegate = site.Target, CallSite = site };
        }

        #endregion


        #region Private CallSite Binder Factories

        private static Tuple<CallSite, Delegate, Func<CallSite, object, Delegate, object[], object>> CreateDynamicInvokerBinderHelper<T>(CallSiteBinder binder, Func<CallSite, object, Delegate, object[], object> argsAction)
            where T : class
        {
            var mySite = CallSite<T>.Create(binder);

            return new Tuple<CallSite, Delegate, Func<CallSite, object, Delegate, object[], object>>(
                mySite,
                mySite.Target as Delegate, argsAction);
        }

        private static Tuple<CallSite, Delegate, Func<CallSite, object, Delegate, object[], object>> CreateDynamicInvokerBinderAndInvokerFunc(string methodName, int argsCount)
        {

            CallSiteBinder binder = Binder.InvokeMember(
                CSharpBinderFlags.None,
                methodName,
                null,
                typeof(LateBindingHelpers),
                // make sure that we have an arg declaration for every type argument in the callsite binder
                // 1 (object type)
                // 1 (return type)
                // 1+1=2
                GetEmptyCSharpArgs(argsCount + 2));



            switch (argsCount)
            {
                case 0:
                    return CreateDynamicInvokerBinderHelper<Func<CallSite, object, object>>(binder,
                        (c, t, d, a) => (d as Func<CallSite, object, object>).Invoke(c, t));
                case 1:
                    return CreateDynamicInvokerBinderHelper<Func<CallSite, object, object, object>>(binder,
                        (c, t, d, a) => (d as Func<CallSite, object, object, object>).Invoke(c, t, a[0]));
                case 2:
                    return CreateDynamicInvokerBinderHelper<Func<CallSite, object, object, object, object>>(binder,
                        (c, t, d, a) => (d as Func<CallSite, object, object, object, object>).Invoke(c, t, a[0], a[1]));
                case 3:
                    return CreateDynamicInvokerBinderHelper<Func<CallSite, object, object, object, object, object>>(binder,
                        (c, t, d, a) => (d as Func<CallSite, object, object, object, object, object>).Invoke(c, t, a[0], a[1], a[2]));
                case 4:
                    return CreateDynamicInvokerBinderHelper<Func<CallSite, object, object, object, object, object, object>>(binder,
                        (c, t, d, a) => (d as Func<CallSite, object, object, object, object, object, object>).Invoke(c, t, a[0], a[1], a[2], a[3]));
                case 5:
                    return CreateDynamicInvokerBinderHelper<Func<CallSite, object, object, object, object, object, object, object>>(binder,
                        (c, t, d, a) => (d as Func<CallSite, object, object, object, object, object, object, object>).Invoke(c, t, a[0], a[1], a[2], a[3], a[4]));
                case 6:
                    return CreateDynamicInvokerBinderHelper<Func<CallSite, object, object, object, object, object, object, object, object>>(binder,
                        (c, t, d, a) => (d as Func<CallSite, object, object, object, object, object, object, object, object>).Invoke(c, t, a[0], a[1], a[2], a[3], a[4], a[5]));
                //TODO: When incrementing args, be sure to add an object type arguement, and cast the delegate to the appropriate func type
                default:

                    throw new NotImplementedException();
            }




        }


        private static CallSite<Func<CallSite, object, object, object>> CreateDynamicInvokerBinder(string methodName, object arg1)
        {

            CallSiteBinder binder = Binder.InvokeMember(
                CSharpBinderFlags.None,
                methodName,
                null,
                typeof(LateBindingHelpers),
                GetEmptyCSharpArgs(3));

            var site = CallSite<Func<CallSite, object, object, object>>.Create(binder);
            return site;
        }

        //private static CallSite<Func<CallSite, object, object>> CreateReturnMethodInvokeCallSiteBinder(string methodName)
        //{

        //    CallSiteBinder binder = Binder.InvokeMember(
        //        CSharpBinderFlags.None,
        //        methodName, null, typeof(LateBindingHelpers),
        //        GetEmptyCSharpArgs(2));

        //    var site = CallSite<Func<CallSite, object, object>>.Create(binder);
        //    return site;
        //}


        private static CallSite<Func<CallSite, object, object>> CreatePropertyGetCallSiteBinder(string propertyName)
        {
            var args = GetEmptyCSharpArgs(1);
            CallSiteBinder binder = Binder.GetMember(CSharpBinderFlags.None, propertyName, typeof(LateBindingHelpers), args);

            var site = CallSite<Func<CallSite, object, object>>.Create(binder);
            return site;
        }

        private static CallSite<Func<CallSite, object, TValue, object>> CreatePropertySetCallSiteBinder<TValue>(string propertyName)
        {
            var args = GetEmptyCSharpArgs(2);
            CallSiteBinder binder = Binder.SetMember(CSharpBinderFlags.None, propertyName, typeof(LateBindingHelpers), args);
            var site = CallSite<Func<CallSite, object, TValue, object>>.Create(binder);
            return site;
        }

        #endregion


        #region Direct Access


        public static TValue GetDynamicValue<TValue>(object o, string propertyName)
        {
            var q = CreateDynamicGetInvoker(propertyName);
            return (TValue)q.Invoke(o);
        }

        public static void SetDynamicValue<TValue>(object o, string propertyName, TValue value)
        {
            var q = CreateDynamicSetInvoker<TValue>(propertyName);
            q.Invoke<TValue>(o, value);
        }

        #endregion

    }
}
