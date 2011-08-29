using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GeniusCode.Components.Extensions
{
    public static class ObjectExtensions
    {

        public static bool TryAs<T>(this object input, Action<T> toTry)
        where T : class
        {
            Action<Exception> onError = null;
            Action onFinally = null;
            return TryAs<T>(input, toTry, onError, onFinally);
        }



        public static bool TryAs<T>(this object input, Action<T> toTry, Action<Exception> onError, Action onFinally)
            where T : class
        {
            T caster = input as T;
            if (caster == null)
                return false;


            Try<T>(caster, toTry, onError, onFinally);
            return true;
        }

        public static bool TryAs<T, T2>(this object input, object input2, Action<T, T2> toTry, Action<Exception> onError, Action onFinally)
            where T : class
            where T2 : class
        {
            T caster = input as T;
            T2 caster2 = input2 as T2;
            if (caster == null || caster2 == null)
                return false;


            Try<T>(caster, (a) => toTry(a, caster2), onError, onFinally);
            return true;
        }

        public static void Try<T>(this T input, Action<T> toTry, Action<Exception> onError, Action onFinally)
        {
            try
            {
                toTry(input);
            }
            catch (Exception ex)
            {
                if (onError != null)
                    onError(ex);
                else
                    throw ex;
            }
            finally
            {
                if (onFinally != null)
                    onFinally();
            }
        }
    }
}
