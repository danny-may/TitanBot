using Discord;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TitanBot.Commands
{
    public class Embedable : IEmbedable
    {
        public EmbedBuilder Builder { get; }
        private Func<Embed, string> StringFormatter { get; }

        private Embedable(EmbedBuilder value, Func<IEmbed, string> stringFormat)
        {
            Builder = value;
            StringFormatter = stringFormat;
        }

        public Embed GetEmbed()
            => Builder;

        public string GetString()
            => StringFormatter(Builder);

        public static implicit operator Embedable(Embed embed)
            => FromEmbed(embed);
        public static implicit operator Embedable(EmbedBuilder embed)
            => FromEmbed(embed);

        public static Embedable FromEmbed(EmbedBuilder builder)
            => FromEmbed(builder, GetString);
        public static Embedable FromEmbed(Embed embed)
            => FromEmbed(GetBuilder(embed));
        public static Embedable FromEmbed(Embed embed, Func<Embed, string> stringFormat)
            => FromEmbed(GetBuilder(embed), stringFormat);

        public static Embedable FromEmbed(EmbedBuilder builder, Func<IEmbed, string> stringFormat)
            => builder == null ? null : new Embedable(builder, stringFormat);

        private static EmbedBuilder GetBuilder(IEmbed embed)
        {
            if (embed == null)
                return null;
            var builder = new EmbedBuilder();
            if (embed.Author != null)
                builder.WithAuthor(new EmbedAuthorBuilder
                {
                    IconUrl = embed.Author?.IconUrl,
                    Name = embed.Author?.Name,
                    Url = embed.Author?.Url
                });
            if (embed.Color != null) builder.WithColor(embed.Color.Value);
            builder.WithDescription(embed.Description);
            if (embed.Footer != null)
                builder.WithFooter(new EmbedFooterBuilder
                {
                    IconUrl = embed.Footer?.IconUrl,
                    Text = embed.Footer?.Text
                });
            if (embed.Image != null) builder.WithImageUrl(embed.Image?.Url);
            if (embed.Thumbnail != null) builder.WithThumbnailUrl(embed.Thumbnail?.Url);
            if (embed.Timestamp != null) builder.WithTimestamp(embed.Timestamp.Value);
            builder.WithTitle(embed.Title);
            builder.WithUrl(embed.Url);

            return builder;
        }
        private static string GetString(Embed embed)
        {
            var blocks = new List<string>();
            if (embed.Author != null) blocks.Add($"{embed.Author?.Name}\n{embed.Author?.Url}".Trim());
            if (embed.Title != null) blocks.Add($"**{embed.Title}**".Trim());
            if (embed.Description != null) blocks.Add(embed.Description.Trim());
            if (embed.Thumbnail != null) blocks.Add($"Thumbnail: {embed.Thumbnail?.Url}".Trim());
            foreach (var field in embed.Fields)
                blocks.Add($"**{field.Name.Trim()}**\n{field.Value.ToString().Trim()}");
            if (embed.Image != null) blocks.Add(embed.Image?.Url.Trim());
            blocks.Add(string.Join(" | ", new object[] { embed.Footer?.Text, embed.Timestamp }.Where(v => !string.IsNullOrWhiteSpace(v?.ToString()))).Trim());

            return string.Join("\n---------\n", blocks.Where(b => !string.IsNullOrWhiteSpace(b)).ToArray());
        }
    }
}
