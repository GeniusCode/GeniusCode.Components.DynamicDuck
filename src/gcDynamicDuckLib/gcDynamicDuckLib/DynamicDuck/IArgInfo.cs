using System;

namespace GeniusCode.Components.DynamicDuck
{
    public interface IArgInfo
    {
        string ArguementName { get; }
        object ArguementValue { get; }
        Type ArguementType { get; }
    }
}