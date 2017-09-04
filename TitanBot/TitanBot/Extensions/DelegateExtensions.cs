using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class DelegateExtensions
    {
        public static T RunInline<T>(this Action<T> action, T target)
        {
            action?.Invoke(target);
            return target;
        }
    }
}