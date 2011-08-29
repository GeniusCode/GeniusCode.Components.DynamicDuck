using System;

namespace GeniusCode.Components.DynamicDuck.Support
{
    internal class ArgInfo<T> : IArgInfo
    {

        public ArgInfo(string name, T value)
        {
            ArguementValue = value;
            ArguementName = name;
        }

        public T ArguementValue { get; private set; }
        public Type ArguementType
        {
            get
            {
                return typeof(T);
            }
        }
        public string ArguementName { get; private set; }

        string IArgInfo.ArguementName
        {
            get { return ArguementName; }
        }

        object IArgInfo.ArguementValue
        {
            get { return ArguementValue; }
        }
    }
}
