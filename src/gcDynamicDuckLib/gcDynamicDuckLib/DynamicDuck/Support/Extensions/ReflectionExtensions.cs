using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GeniusCode.Components.Extensions;

namespace GeniusCode.Components.DynamicDuck.Support.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool IsVoidReturnType(this MethodInfo methodInfo)
        {
            return methodInfo.ReturnType == typeof(void);
        }

        public static bool HasAttribute<TAttribute>(this Type input, bool includeInheritance)
where TAttribute : Attribute
        {
            return GetAttribute<TAttribute>(input, includeInheritance) != null;
        }
        public static TAttribute GetAttribute<TAttribute>(this Type input, bool includeInheritance) where TAttribute : Attribute
        {
            var attribute = input.GetCustomAttributes<TAttribute>(includeInheritance).SingleOrDefault();
            return attribute;
        }

        public static List<T> GetCustomAttributes<T>(this MemberInfo input, bool includeInheritance)
            where T : Attribute
        {
            return input.GetCustomAttributes(typeof(T), includeInheritance).Cast<T>().ToList(); 
        }

        public static IEnumerable<MethodInfo> WhereExplicitMethodDefinitions(this MethodInfo[] infos)
        {
            string[] prefixes = {
	                                "get_",
	                                "set_",
	                                "add_",
	                                "remove_"
	                                };
            return infos.Where(x => !x.Name.ContainsAnyWords(prefixes));
        }

    }
}
