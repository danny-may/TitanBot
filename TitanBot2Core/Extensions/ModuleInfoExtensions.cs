using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Extensions
{
    public static class ModuleInfoExtensions
    {
        public static ModuleInfo GetTopParent(this ModuleInfo module, int depth = 0)
        {
            var stack = GetParentStack(module);
            return stack.Skip(depth).FirstOrDefault();
        }

        public static ModuleInfo GetTopParent(this CommandInfo command, int depth = 0)
        {
            return command.Module.GetTopParent(depth);
        }

        private static List<ModuleInfo> GetParentStack(ModuleInfo module)
        {
            List<ModuleInfo> modules;
            if (module.IsSubmodule)
                modules = GetParentStack(module.Parent);
            else
                modules = new List<ModuleInfo>();

            modules.Add(module);
            return modules;
        }

        public static string FindSummary(this CommandInfo cmd)
        {
            var summary = cmd.Summary;
            var looking = cmd.Module;
            while (string.IsNullOrWhiteSpace(summary) && looking.IsSubmodule)
            {
                summary = looking.Summary;
                looking = looking.Parent;
            }

            return summary;
        }

        public static string ToHelpString(this ParameterInfo parameter)
        {
            string template;
            if (parameter.IsOptional || parameter.IsMultiple)
                template = "[{0}]";
            else
                template = "<{0}>";

            if (parameter.IsMultiple)
                return string.Format(template, $"{parameter.Name}1, {parameter.Name}2, {parameter.Name}3...");
            else
                return string.Format(template, parameter.Name);
        }
    }
}
