using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Formatting
{
    public struct FormattingType
    {
        public static Dictionary<FormattingType, string> Names { get; } = new Dictionary<FormattingType, string> { { DEFAULT, "Default" } };

        public static FormattingType DEFAULT { get; } = 0;
        public static FormattingType UNKNOWN { get; } = -1;

        public int Id { get; }
        public string Name => Names[this];

        internal FormattingType(int type)
        {
            Id = type;
        }
        
        public override string ToString()
            => Name;

        public bool Equals(FormattingType other)
            => Id == other.Id;

        public static implicit operator FormattingType(int id)
            => new FormattingType(id);

        public  static implicit operator int(FormattingType format)
            => format.Id;

        public static implicit operator FormattingType(string name)
            => Names.ContainsValue(name) ? Names.First(n => n.Value == name).Key : UNKNOWN;

        public static bool operator ==(FormattingType format1, FormattingType format2)
            => format1.Id == format2.Id;

        public static bool operator !=(FormattingType format1, FormattingType format2)
            => format1.Id != format2.Id;

        public override bool Equals(object obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => base.GetHashCode();

    }
}
