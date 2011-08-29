using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeniusCode.Components.DynamicDuck;
using GeniusCode.Components.DynamicDuck.Support;

namespace GcCore.Tests.Dynamic
{
    public interface ITestClass//<R> where R : struct
    {
        string TwoWayProperty { get; set; }
        string ReadOnlyProperty { get; }
        string WriteOnlyProperty { set; }
        event EventHandler DefaultEvent;
        event EventHandler<EventArgs> DefaultEvent2;
        event PropertyChangedEventHandler PropertyChangedEvent;

        //void VoidMethod(bool input1, string input2);
        //void VoidMethod2();
        //int ResultMethod(bool input1, string input2);
        T resultMethod<T>(int input1, bool input2, List<T> input3, Lazy<T> input4) where T : class, INotifyPropertyChanged, new();
        //void resultMethod2<T>(int input1, bool input2, T input3);
    }

    public interface ISampleClass
    {
        string TwoWayProperty { get; set; }
        string ReadOnlyProperty { get; }
        string WriteOnlyProperty { set; }

        void VoidMethod(bool input1, string input2);
        void VoidMethod2();
        int ResultMethod(bool input1, string input2);
        T resultMethod<T>(int input1, bool input2, T input3);
        void resultMethod2<T>(int input1, bool input2, T input3);

        //event EventHandler SampleEvent;
    }

    class SampleClass : DynamicProxyBase, ISampleClass
    {
        /// <summary>
        /// Creates a new dynamic object wrapping the specified <paramref name="target">target object</paramref>.
        /// </summary>
        /// <param name="target">Wrapped target object.</param>
        protected SampleClass(object target, IDynamicInteractionProvider provider)
            : base(target, provider)
        {
        }


        string ISampleClass.TwoWayProperty
        {
            get
            {
                return base.GetProperty<string>("ISampleClass.TwoWayProperty");
            }
            set
            {
                base.SetProperty<string>("ISampleClass.TwoWayProperty", value);
            }
        }

        string ISampleClass.ReadOnlyProperty
        {
            get
            {
                return base.GetProperty<string>("ISampleClass.ReadOnlyProperty");
            }
        }

        string ISampleClass.WriteOnlyProperty
        {
            set
            {
                base.SetProperty<string>("ISampleClass.WriteOnlyProperty", value);
            }
        }

        void ISampleClass.VoidMethod(bool input1, string input2)
        {
            base.InvokeVoidMethod("ISampleClass.VoidMethod", GetArgInfo<bool>("input1", input1), GetArgInfo<string>("input2", input2));
        }

        int ISampleClass.ResultMethod(bool input1, string input2)
        {
            return InvokeReturnMethod<int>("ISampleClass.ResultMethod", GetArgInfo<bool>("input1", input1), GetArgInfo<string>("input2", input2));
        }

        //event EventHandler ISampleClass.SampleEvent
        //{
        //    add { base.AddHandler("ISampleClass.SampleEvent", value); }
        //    remove { base.RemoveHandler("ISampleClass.SampleEvent", value); }
        //}


        void ISampleClass.VoidMethod2()
        {
            base.InvokeVoidMethod("ISampleClass.VoidMethod2", null);
        }


        T ISampleClass.resultMethod<T>(int input1, bool input2, T input3)
        {
            return base.InvokeReturnMethod<T>("ISampleClass.resultMethod'T",
                GetArgInfo<int>("input1", input1), GetArgInfo<bool>("input2", input2), GetArgInfo<T>("input3", input3));
        }

        void ISampleClass.resultMethod2<T>(int input1, bool input2, T input3)
        {
            base.InvokeVoidMethod("ISampleClass.resultMethod'T",
                GetArgInfo<int>("input1", input1), GetArgInfo<bool>("input2", input2), GetArgInfo<T>("input3", input3));
        }
    }
}
