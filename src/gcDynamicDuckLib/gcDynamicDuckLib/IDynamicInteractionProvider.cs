using System;
using System.Reflection;

namespace GeniusCode.Components.DynamicDuck
{
    public class MethodCallSiteInfo
    {
        public object Target { get; set; }
        public string MethodName { get; set; }
        public IArgInfo[] Args { get; set; }
    }

    /// <summary>
    /// Provider that supports ducking (treating an existing object as though it implements an interface)
    /// </summary>
    public interface IDuckInteractionProvider : IDynamicInteractionProvider { }

    /// <summary>
    /// Provider that supports mocking (creating a new expando, and treating it as though it implements an interface)
    /// </summary>
    public interface IMockInteractionProvider : IDynamicInteractionProvider { }

    /// <summary>
    /// Core provider class that is used by both Ducking and Mocking
    /// </summary>
    public interface IDynamicInteractionProvider
    {
        void OnObjectWrapped<T>(T target);
        //TODO: Add type array of arguements
        void InvokeVoidMethod(MethodCallSiteInfo info);
        T InvokeReturnMethod<T>(MethodCallSiteInfo info);

        void AddHandler(object target, string eventName, Delegate handler, Type delegateType);
        void RemoveHandler(object target, string eventName, Delegate handler, Type delegateType);

        void PerformPropertySet<T>(T value, object target, string propertyName);
        T PerformPropertyGet<T>(object target, string propertyName);
        bool CachePoints { get; set; }
        void SetDefaultValue<T>(T value, object target, string propertyName);

        string GetQualifiedName(MemberInfo originalMember);
    }
}