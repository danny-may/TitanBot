using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Formatting;
using TitanBot.Formatting.Interfaces;
using TitanBot.Replying;

namespace TitanBot.Commands
{
    public class Embedable : IEmbedable
    {
        public ILocalisable<EmbedBuilder> Builder { get; }
        private Func<ILocalisable<EmbedBuilder>, ILocalisable<string>> StringFormatter { get; }

        private Embedable(ILocalisable<EmbedBuilder> value, Func<ILocalisable<EmbedBuilder>, ILocalisable<string>> stringFormat)
        {
            Builder = value;
            StringFormatter = stringFormat;
        }

        public ILocalisable<EmbedBuilder> GetEmbed()
            => Builder;

        public ILocalisable<string> GetString()
            => StringFormatter(Builder);

        public static implicit operator Embedable(Embed embed)
            => FromEmbed(GetBuilder(embed), GetString);
        public static implicit operator Embedable(EmbedBuilder embed)
            => FromEmbed(embed, GetString);
        public static implicit operator Embedable(LocalisedEmbedBuilder embed)
            => FromEmbed(embed, GetString);

        public static Embedable FromEmbed(Embed embed)
            => FromEmbed(GetBuilder(embed), GetString);
        public static Embedable FromEmbed(ILocalisable<EmbedBuilder> embed)
            => FromEmbed(embed, GetString);
        public static Embedable FromEmbed(Embed embed, Func<ILocalisable<EmbedBuilder>, ILocalisable<string>> stringFormat)
            => FromEmbed(GetBuilder(embed), stringFormat);
        public static Embedable FromEmbed(ILocalisable<EmbedBuilder> embed, Func<ILocalisable<EmbedBuilder>, ILocalisable<string>> stringFormat)
            => embed == null ? null : new Embedable(embed, stringFormat);

        private static ILocalisable<EmbedBuilder> GetBuilder(Embed embed)
        {
            if (embed == null)
                return null;
            var builder = new LocalisedEmbedBuilder();
            if (embed.Author != null)
                builder.WithAuthor(new EmbedAuthorBuilder
                {
                    IconUrl = embed.Author?.IconUrl,
                    Name = embed.Author?.Name,
                    Url = embed.Author?.Url
                });
            if (embed.Color != null) builder.WithColor(embed.Color.Value);
            builder.WithRawDescription(embed.Description);
            if (embed.Footer != null)
                builder.WithFooter(new EmbedFooterBuilder
                {
                    IconUrl = embed.Footer?.IconUrl,
                    Text = embed.Footer?.Text
                });
            if (embed.Image != null) builder.WithRawImageUrl(embed.Image?.Url);
            if (embed.Thumbnail != null) builder.WithRawThumbnailUrl(embed.Thumbnail?.Url);
            if (embed.Timestamp != null) builder.WithTimestamp(embed.Timestamp.Value);
            builder.WithRawTitle(embed.Title);
            builder.WithRawUrl(embed.Url);

            return builder;
        }
        private static ILocalisable<string> GetString(ILocalisable<EmbedBuilder> embed)
        {
            return new DynamicString(tr =>
            {
                var localised = embed.Localise(tr);
                var blocks = new List<string>();
                if (localised.Author != null) blocks.Add($"{localised.Author?.Name}\n{localised.Author?.Url}".Trim());
                if (localised.Title != null) blocks.Add($"**{localised.Title}**".Trim());
                if (localised.Description != null) blocks.Add(localised.Description.Trim());
                if (localised.ThumbnailUrl != null) blocks.Add($"Thumbnail: {localised.ThumbnailUrl}".Trim());
                foreach (var field in localised.Fields)
                    blocks.Add($"**{field.Name.Trim()}**\n{field.Value.ToString().Trim()}");
                if (localised.ImageUrl != null) blocks.Add(localised.ImageUrl.Trim());
                blocks.Add(string.Join(" | ", new object[] { localised.Footer?.Text, localised.Timestamp }.Where(v => !string.IsNullOrWhiteSpace(v?.ToString()))).Trim());

                return string.Join("\n---------\n", blocks.Where(b => !string.IsNullOrWhiteSpace(b)).ToArray());
            });
        }
    }
}
