namespace TitanBot.Core.Models
{
    public class BoundValue<TValue> : BoundModel
    {
        #region Fields

        private string _key;
        private TValue _value;

        public string Key => _key;

        public TValue Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        #endregion Fields

        #region Constructors

        public BoundValue(string key, TValue value)
        {
            _key = key;
            _value = value;
        }

        #endregion Constructors

        #region Operators

        public static implicit operator TValue(BoundValue<TValue> bound)
            => bound.Value;

        #endregion Operators
    }
}