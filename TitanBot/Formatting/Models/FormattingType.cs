using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Formatting
{
    public struct FormattingType
    {
        public static FormattingType DEFAULT { get; } = 0;

        private uint _id { get; }

        internal FormattingType(uint type)
        {
            _id = type;
        }

        public bool Equals(FormattingType other)
            => _id == other._id;

        public static implicit operator FormattingType(uint id)
            => new FormattingType(id);

        public  static implicit operator uint(FormattingType format)
            => format._id;

        public static bool operator ==(FormattingType format1, FormattingType format2)
            => format1._id == format2._id;

        public static bool operator !=(FormattingType format1, FormattingType format2)
            => format1._id != format2._id;

        public override bool Equals(object obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => base.GetHashCode();

    }
}
