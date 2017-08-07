using System.Linq;

namespace TitanBot.Models
{
    public class Range<T>
    {
        protected delegate bool TryParseDelegate(string text, out T value);

        public T From { get; set; }
        public T To { get; set; }

        public T Min => new T[] { From, To }.Min();
        public T Max => new T[] { From, To }.Max();

        public static implicit operator Range<T>((T From, T to) tuple)
            => new Range<T> { From = tuple.From, To = tuple.to };
    }
}
