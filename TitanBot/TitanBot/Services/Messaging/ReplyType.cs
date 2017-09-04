using System;
using System.Linq;
using TitanBot.Core.Services.Formatting;
using TitanBot.Core.Services.Formatting.Models;

namespace TitanBot.Services.Messaging
{
    public class ReplyType : FunctionalEnum<ReplyType, int>, IDisplayable<string>
    {
        public static readonly ReplyType Error = new ReplyType(nameof(Error));
        public static readonly ReplyType Info = new ReplyType(nameof(Info));
        public static readonly ReplyType None = new ReplyType(nameof(None));
        public static readonly ReplyType Success = new ReplyType(nameof(Success));

        public IDisplayable<string> ToLocalisable()
            => TransKey.From($"REPLYTYPE_{this}".ToUpper());

        object IDisplayable.Display(ITranslationSet translations, IValueFormatter formatter)
            => Display(translations, formatter);

        public string Display(ITranslationSet translations, IValueFormatter formatter)
            => ToLocalisable().Display(translations, formatter);

        private ReplyType(int key, string name) : base(key, name)
        {
        }

        private ReplyType(string name) : base(Next, name)
        {
        }

        private static int Next => enumValues.Max(e => e.Key) + 1;
    }
}