using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Extensions
{
    public static class TaskExtensions
    {
        public static void AllowRun(this Task t) { }
        public static void AllowRun<T>(this Task<T> t) { }
    }
}
