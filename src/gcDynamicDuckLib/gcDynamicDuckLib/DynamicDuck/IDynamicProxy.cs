
namespace GeniusCode.Components.DynamicDuck
{
    public interface IDynamicProxy
    {
        /// <summary>
        /// Expando
        /// </summary>
        object Target { get; }
        IDynamicInteractionProvider Provider { get; }

    }
}
