namespace TitanBot.Core.Services.TypeReader
{
    public sealed class TypeReaderMatch : ITypeReaderMatch
    {
        public double Certainty { get; }
        public object Value { get; }

        public TypeReaderMatch(double certainty, object value)
        {
            Certainty = certainty;
            Value = value;
        }

        public override string ToString()
            => $"{Certainty}% : {Value}";
    }
}