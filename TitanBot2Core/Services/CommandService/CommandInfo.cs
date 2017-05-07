using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot2.TypeReaders;

namespace TitanBot2.Services.CommandService
{
    public class CommandInfo
    {
        public Type CommandType { get; private set; }
        public string Group { get; private set; }
        public string Name { get; private set; }
        public string[] Alias { get; private set; }
        public string Description { get; private set; }
        public string[] Usage { get; private set; }

        private CommandInfo()
        {
            
        }

        public Command CreateInstance(TitanbotCmdContext context, TypeReaderCollection readers)
        {
            return Activator.CreateInstance(CommandType, context, readers) as Command;
        }

        public static CommandInfo[] FromType(Type t)
        {
            if (t.IsSubclassOf(typeof(Command)) && t.IsClass && !t.IsAbstract)
            {
                try
                {
                    var obj = Activator.CreateInstance(t, null as TitanbotCmdContext, null as TypeReaderCollection) as Command;
                    if (obj == null)
                        return new CommandInfo[0];

                    var names = new List<string>();
                    names.Add(obj.Name ?? t.Name);
                    names.AddRange(obj.Alias ?? new string[0]);

                    return names.Select(n => new CommandInfo
                    {
                        Name = n,
                        CommandType = t,
                        Group = obj.Group ?? t.Namespace.Split('.').Last(),
                        Alias = obj.Alias.Concat(new string[] { obj.Name }).ToArray(),
                        Description = obj.Description,
                        Usage = obj.Usage
                    }).ToArray();

                } catch { }
            }

            return new CommandInfo[0];
        }
    }
}
