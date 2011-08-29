namespace GeniusCode.Components.DynamicDuck
{
    public class MethodCallSiteInfo
    {
        public object Target { get; set; }
        public string MethodName { get; set; }
        public IArgInfo[] Args { get; set; }
    }
}