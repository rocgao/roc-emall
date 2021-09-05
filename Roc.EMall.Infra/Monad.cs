using System;

namespace Roc.EMall.Infra
{
    public static class Monad
    {
        public static TU Map<T,TU>(this T t,Func<T,TU> func)
        {
            return func(t);
        }

        public static void Foreach<T>(this T[] items, Action<T> func)
        {
            foreach (var item in items)
            {
                func(item);
            }
        }
    }
}