using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TitanBot.TextResource;

namespace TitanBot.TypeReaders
{
    public struct TypeReaderValue
    {
        public object Value { get; }
        public float Score { get; }

        public TypeReaderValue(object value, float score)
        {
            Value = value;
            Score = score;
        }

        public override string ToString() => Value?.ToString();
    }

    public struct TypeReaderResponse
    {
        public IReadOnlyCollection<TypeReaderValue> Values { get; }
        
        public (string message, Func<ITextResourceCollection, object>[] values) Message { get; }
        public bool IsSuccess { get; }
        public object Best => Values?.OrderByDescending(v => v.Score).Select(v => v.Value).FirstOrDefault();

        private TypeReaderResponse(IReadOnlyCollection<TypeReaderValue> values, bool success, (string message, Func<ITextResourceCollection, object>[] values) message)
        {
            Values = values;
            Message = message;
            IsSuccess = success;
        }

        public static TypeReaderResponse FromSuccess(object value)
            => new TypeReaderResponse(ImmutableArray.Create(new TypeReaderValue(value, 1.0f)), true, (null, null));
        public static TypeReaderResponse FromSuccess(TypeReaderValue value)
            => new TypeReaderResponse(ImmutableArray.Create(value), true, (null, null));
        public static TypeReaderResponse FromSuccess(IReadOnlyCollection<TypeReaderValue> values)
            => new TypeReaderResponse(values, true, (null, null));
        public static TypeReaderResponse FromError(string message, string value, Type target, params object[] other)
            => new TypeReaderResponse(null, false, (message, BuildFuncs(value, target, other)));

        private static Func<ITextResourceCollection, object>[] BuildFuncs(string value, Type target, object[] other)
        {
            var vals = new List<Func<ITextResourceCollection, object>>
            {
                t => value,
                t => t.GetResource(target.Name)
            };
            foreach (var item in other)
                vals.Add(t => item);

            return vals.ToArray();
        }

        public override string ToString() => IsSuccess.ToString();
    }
}
