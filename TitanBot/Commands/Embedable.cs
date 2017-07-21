using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Util;

namespace TitanBot.Commands
{
    public class Embedable : IEmbedable
    {
        public EmbedBuilder Builder { get; }
        private Func<Embed, string> StringFormatter { get; }

        private Embedable(EmbedBuilder value, Func<Embed, string> stringFormat)
        {
            Builder = value;
            StringFormatter = stringFormat;
        }

        public Embed GetEmbed()
            => Builder;

        public string GetString()
        {
            if (StringFormatter != null)
                return StringFormatter(Builder);

            var blocks = new List<string>();
            Builder.Author.CheckNull(a => blocks.Add($"{a.Name}\n{a.Url}".Trim()));
            Builder.Description.CheckNull(d => blocks.Add(d.Trim()));
            Builder.ThumbnailUrl.CheckNull(t => blocks.Add($"Thumbnail: {t}".Trim()));
            foreach (var field in Builder.Fields)
                blocks.Add($"**{field.Name.Trim()}**\n{field.Value.ToString().Trim()}");
            Builder.ImageUrl.CheckNull(i => blocks.Add(i.Trim()));
            blocks.Add(string.Join(" | ", new object[] { Builder.Footer?.Text, Builder.Timestamp }.Where(v => !string.IsNullOrWhiteSpace(v?.ToString()))).Trim());

            return string.Join("\n---------\n", blocks.Where(b => !string.IsNullOrWhiteSpace(b)).ToArray());
        }

        public static Embedable FromEmbed(EmbedBuilder builder)
            => new Embedable(builder, null);
        public static Embedable FromEmbed(EmbedBuilder builder, Func<Embed, string> stringFormat)
            => new Embedable(builder, stringFormat);
    }
}
