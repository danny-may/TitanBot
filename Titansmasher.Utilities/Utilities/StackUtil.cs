using System;
using System.Diagnostics;
using System.Reflection;

namespace Titansmasher.Utilities
{
    public static class StackUtil
    {
        public static Type GetCallerClass()
            => GetCaller().DeclaringType;

        public static MethodBase GetCaller()
            => new StackFrame(2).GetMethod();
    }
}