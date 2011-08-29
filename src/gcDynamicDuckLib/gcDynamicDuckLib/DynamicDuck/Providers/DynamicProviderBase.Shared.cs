using System;
using System.Reflection;

namespace GeniusCode.Components.DynamicDuck.Providers
{
    public abstract class DynamicProviderBase : IDynamicInteractionProvider
    {
        protected DynamicProviderBase()
        {
            CachePoints = true;
        }

        #region Implementations

        void IDynamicInteractionProvider.OnObjectWrapped<T>(T target)
        {
            OnObjectWrapped(target);
        }

        bool IDynamicInteractionProvider.CachePoints
        {
            get
            {
                return CachePoints;
            }
            set
            {
                CachePoints = value;
            }
        }



        string IDynamicInteractionProvider.GetQualifiedName(MemberInfo originalMember)
        {
            return GetQualifiedName(originalMember);
        }



        void IDynamicInteractionProvider.SetDefaultValue<T>(T value, object target, string propertyName)
        {
            if (CalculateShouldSetDefaultValue(value, target, propertyName))
                PerformPropertySet(target, propertyName, value);
        }

        void IDynamicInteractionProvider.AddHandler(object target, string eventName, Delegate handler, Type delegateType)
        {
            AddHandler(target, eventName, handler, delegateType);
        }

        void IDynamicInteractionProvider.RemoveHandler(object target, string eventName, Delegate handler, Type delegateType)
        {
            RemoveHandler(target, eventName, handler, delegateType);
        }

        void IDynamicInteractionProvider.InvokeVoidMethod(MethodCallSiteInfo info)
        {
            InvokeVoidMethod(info);
        }

        T IDynamicInteractionProvider.InvokeReturnMethod<T>(MethodCallSiteInfo info)
        {
            return InvokeReturnMethod<T>(info);
        }

        void IDynamicInteractionProvider.PerformPropertySet<T>(T value, object target, string propertyName)
        {
            PerformPropertySet(target, propertyName, value);
        }

        T IDynamicInteractionProvider.PerformPropertyGet<T>(object target, string propertyName)
        {
            return PerformPropertyGet<T>(target, propertyName);
        }
        #endregion

        #region abstract members

        protected abstract void InvokeVoidMethod(MethodCallSiteInfo info);
        protected abstract T InvokeReturnMethod<T>(MethodCallSiteInfo info);
        protected abstract void PerformPropertySet<T>(object target, string propertyName, T value);
        protected abstract T PerformPropertyGet<T>(object target, string propertyName);

        #endregion

        public bool CachePoints
        {
            get;
            set;
        }


        #region Virtual Members
        // events are virtual now, because we don't know how to implement yet
        protected virtual void AddHandler(object target, string name, Delegate handler, Type delegateType) { }
        // events are virtual now, because we don't know how to implement yet
        protected virtual void RemoveHandler(object target, string name, Delegate handler, Type delegateType) { }
        

        protected virtual string GetQualifiedName(MemberInfo member)
        {
            return member.Name;
        }

        protected virtual bool CalculateShouldSetDefaultValue<T>(T value, object target, string propertyName)
        {
            return true;
        }

        protected virtual void OnObjectWrapped<T>(T target)
        {
        }

        #endregion




    }
}
