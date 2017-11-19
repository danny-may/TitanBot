using System.Collections.Generic;
using System.Linq;
using Titansmasher.Services.Display.Interfaces;

namespace Titansmasher.Services.Display
{
    public class Format : IDisplayable<string>
    {
        #region Statics

        private static HashSet<Format> _known = new HashSet<Format>();

        public static Format Default { get; } = 0;

        public static Format Get(uint i)
            => _known.FirstOrDefault(f => f == i) ?? new Format(i);

        #endregion Statics

        #region Fields

        public IDisplayable<string> Name => _name;
        public IDisplayable<string> Description => _description;

        private uint _id;
        private Translation _name;
        private Translation _description;

        #endregion Fields

        #region Constructors

        private Format(uint id)
        {
            _id = id;
            _name = new Translation($"format[{id}].name");
            _description = new Translation($"format[{id}].description");
            _known.Add(this);
        }

        #endregion Constructors

        #region Overrides

        public override bool Equals(object obj)
            => (obj is Format f && f._id == _id) ||
               obj.Equals(_id);

        public override string ToString()
            => _name.Key;

        public override int GetHashCode()
            => _id.GetHashCode();

        #endregion Overrides

        #region Operators

        #region Implicit

        public static implicit operator uint(Format f)
            => f._id;

        public static implicit operator Format(uint i)
            => Get(i);

        #endregion Implicit

        #endregion Operators

        #region IDisplayable

        public string Display(IDisplayService service, DisplayOptions options = default)
            => Name.Display(service, options);

        object IDisplayable.Display(IDisplayService service, DisplayOptions options)
            => Display(service, options);

        #endregion IDisplayable
    }
}