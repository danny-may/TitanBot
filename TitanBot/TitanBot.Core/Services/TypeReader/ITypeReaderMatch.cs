namespace TitanBot.Core.Services.TypeReader
{
    public interface ITypeReaderMatch
    {
        double Certainty { get; }
        object Value { get; }
    }
}