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
        public List<string> Alias { get; private set; }
        public string Description { get; private set; }
        public List<string> Usage { get; private set; }

        private CommandInfo()
        {
            
        }

        public Command CreateInstance(CmdContext context, TypeReaderCollection readers)
        {
            return Activator.CreateInstance(CommandType, context, readers) as Command;
        }

        public static CommandInfo FromType(Type t)
        {
            if (t.IsSubclassOf(typeof(Command)) && t.IsClass && !t.IsAbstract)
            {
                try
                {
                    var obj = Activator.CreateInstance(t, null as CmdContext, null as TypeReaderCollection) as Command;
                    if (obj == null)
                        return null;

                    var names = new List<string>();
                    names.Add(obj.Name ?? t.Name);
                    names.AddRange(obj.Alias ?? new List<string>());

                    return new CommandInfo
                    {
                        Name = obj.Name,
                        Alias = obj.Alias.Concat(new List<string> { obj.Name }).ToList(),
                        CommandType = t,
                        Description = obj.Description,
                        Group = obj.Group,
                        Usage = obj.Usage
                    };

                } catch { }
            }

            return null;
        }
    }
}
