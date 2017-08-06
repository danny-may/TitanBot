using Discord;
using Discord.WebSocket;
using Discord.Rest;
using Discord.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Formatting;
using TitanBot.Replying;
using TitanBot.Util;
using TitanBot.Formatting.Interfaces;

namespace TitanBot.Commands
{
    public class LocalisedEmbedBuilder : ILocalisable<EmbedBuilder>
    {

        public LocalisedString Title { get; set; }
        public LocalisedString Description { get; set; }
        public LocalisedString Url { get; set; }
        public LocalisedString ThumbnailUrl { get; set; }
        public LocalisedString ImageUrl { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public Color? Color { get; set; }

        public LocalisedAuthorBuilder Author { get; set; }
        public LocalisedFooterBuilder Footer { get; set; }
        private List<LocalisedFieldBuilder> _fields { get; set; } = new List<LocalisedFieldBuilder>();
        public List<LocalisedFieldBuilder> Fields
        {
            get => _fields;
            set
            {
                if (value == null) throw new ArgumentNullException("Cannot set an embed builders' fields collection to null", nameof(Fields));
                if (value.Count > EmbedBuilder.MaxFieldCount) throw new ArgumentException($"Field count must be less than or equal to {EmbedBuilder.MaxFieldCount}.", nameof(Fields));
                _fields = value;
            }
        }

        public LocalisedEmbedBuilder WithTitle(LocalisedString title)
        {
            Title = title;
            return this;
        }
        public LocalisedEmbedBuilder WithDescription(LocalisedString description)
        {
            Description = description;
            return this;
        }
        public LocalisedEmbedBuilder WithUrl(LocalisedString url)
        {
            Url = url;
            return this;
        }
        public LocalisedEmbedBuilder WithThumbnailUrl(LocalisedString thumbnailUrl)
        {
            ThumbnailUrl = thumbnailUrl;
            return this;
        }
        public LocalisedEmbedBuilder WithImageUrl(LocalisedString imageUrl)
        {
            ImageUrl = imageUrl;
            return this;
        }
        public LocalisedEmbedBuilder WithCurrentTimestamp()
        {
            Timestamp = DateTimeOffset.UtcNow;
            return this;
        }
        public LocalisedEmbedBuilder WithTimestamp(DateTimeOffset? dateTimeOffset)
        {
            Timestamp = dateTimeOffset;
            return this;
        }
        public LocalisedEmbedBuilder WithColor(Color? color)
        {
            Color = color;
            return this;
        }

        public LocalisedEmbedBuilder WithAuthor(LocalisedAuthorBuilder author)
        {
            Author = author;
            return this;
        }
        public LocalisedEmbedBuilder WithAuthor(Action<LocalisedAuthorBuilder> action)
        {
            var author = new LocalisedAuthorBuilder();
            action(author);
            Author = author;
            return this;
        }
        public LocalisedEmbedBuilder WithFooter(LocalisedFooterBuilder footer)
        {
            Footer = footer;
            return this;
        }
        public LocalisedEmbedBuilder WithFooter(Action<LocalisedFooterBuilder> action)
        {
            var footer = new LocalisedFooterBuilder();
            action(footer);
            Footer = footer;
            return this;
        }


        public LocalisedEmbedBuilder AddField(LocalisedString name, LocalisedString value, bool inline = false)
            => AddField(new LocalisedFieldBuilder { Name = name, Value = value, IsInline = inline });
        public LocalisedEmbedBuilder AddInlineField(LocalisedString name, LocalisedString value)
            => AddField(name, value, true);
        public LocalisedEmbedBuilder AddField(LocalisedFieldBuilder field)
        {
            if (Fields.Count >= EmbedBuilder.MaxFieldCount)
            {
                throw new ArgumentException($"Field count must be less than or equal to {EmbedBuilder.MaxFieldCount}.", nameof(field));
            }

            Fields.Add(field);
            return this;
        }
        public LocalisedEmbedBuilder AddField(Action<LocalisedFieldBuilder> action)
        {
            var field = new LocalisedFieldBuilder();
            action(field);
            return AddField(field);
        }
        public LocalisedEmbedBuilder AddInlineField(Action<LocalisedFieldBuilder> action)
        {
            var field = new LocalisedFieldBuilder().WithIsInline(true);
            action(field);
            return AddField(field);
        }
        
        object ILocalisable.Localise(ITextResourceCollection textResource)
            => Localise(textResource);
        public EmbedBuilder Localise(ITextResourceCollection textResource)
            => new EmbedBuilder
            {
                Author = Author?.Localise(textResource),
                Color = Color,
                Description = Description?.Localise(textResource),
                Fields = Fields.Select(f => f?.Localise(textResource)).Where(f => f != null).ToList(),
                Footer = Footer?.Localise(textResource),
                ImageUrl = ImageUrl?.Localise(textResource),
                ThumbnailUrl = ThumbnailUrl?.Localise(textResource),
                Timestamp = Timestamp,
                Title = Title?.Localise(textResource),
                Url = Url?.Localise(textResource)
            };

        #region WithX Overloads
        public LocalisedEmbedBuilder WithRawTitle(string rawText)
            => WithTitle(new RawString(rawText));
        public LocalisedEmbedBuilder WithTitle(string key)
            => WithTitle(new LocalisedString(key));
        public LocalisedEmbedBuilder WithTitle(string key, ReplyType replyType)
            => WithTitle(new LocalisedString(key, replyType));
        public LocalisedEmbedBuilder WithTitle(string key, params object[] values)
            => WithTitle(new LocalisedString(key, values));
        public LocalisedEmbedBuilder WithTitle(string key, ReplyType replyType, params object[] values)
            => WithTitle(new LocalisedString(key, replyType, values));
        public LocalisedEmbedBuilder WithRawDescription(string rawText)
            => WithDescription(new RawString(rawText));
        public LocalisedEmbedBuilder WithDescription(string key)
            => WithDescription(new LocalisedString(key));
        public LocalisedEmbedBuilder WithDescription(string key, ReplyType replyType)
            => WithDescription(new LocalisedString(key, replyType));
        public LocalisedEmbedBuilder WithDescription(string key, params object[] values)
            => WithDescription(new LocalisedString(key, values));
        public LocalisedEmbedBuilder WithDescription(string key, ReplyType replyType, params object[] values)
            => WithDescription(new LocalisedString(key, replyType, values));
        public LocalisedEmbedBuilder WithRawUrl(string rawText)
            => WithUrl(new RawString(rawText));
        public LocalisedEmbedBuilder WithUrl(string key)
            => WithUrl(new LocalisedString(key));
        public LocalisedEmbedBuilder WithUrl(string key, ReplyType replyType)
            => WithUrl(new LocalisedString(key, replyType));
        public LocalisedEmbedBuilder WithUrl(string key, params object[] values)
            => WithUrl(new LocalisedString(key, values));
        public LocalisedEmbedBuilder WithUrl(string key, ReplyType replyType, params object[] values)
            => WithUrl(new LocalisedString(key, replyType, values));
        public LocalisedEmbedBuilder WithRawThumbnailUrl(string rawText)
            => WithThumbnailUrl(new RawString(rawText));
        public LocalisedEmbedBuilder WithThumbnailUrl(string key)
            => WithThumbnailUrl(new LocalisedString(key));
        public LocalisedEmbedBuilder WithThumbnailUrl(string key, ReplyType replyType)
            => WithThumbnailUrl(new LocalisedString(key, replyType));
        public LocalisedEmbedBuilder WithThumbnailUrl(string key, params object[] values)
            => WithThumbnailUrl(new LocalisedString(key, values));
        public LocalisedEmbedBuilder WithThumbnailUrl(string key, ReplyType replyType, params object[] values)
            => WithThumbnailUrl(new LocalisedString(key, replyType, values));
        public LocalisedEmbedBuilder WithRawImageUrl(string rawText)
            => WithImageUrl(new RawString(rawText));
        public LocalisedEmbedBuilder WithImageUrl(string key)
            => WithImageUrl(new LocalisedString(key));
        public LocalisedEmbedBuilder WithImageUrl(string key, ReplyType replyType)
            => WithImageUrl(new LocalisedString(key, replyType));
        public LocalisedEmbedBuilder WithImageUrl(string key, params object[] values)
            => WithImageUrl(new LocalisedString(key, values));
        public LocalisedEmbedBuilder WithImageUrl(string key, ReplyType replyType, params object[] values)
            => WithImageUrl(new LocalisedString(key, replyType, values));
        #endregion
    }

    public class LocalisedAuthorBuilder : ILocalisable<EmbedAuthorBuilder>
    {
        public LocalisedString Name { get; set; }
        public LocalisedString Url { get; set; }
        public LocalisedString IconUrl { get; set; }
        
        public LocalisedAuthorBuilder WithName(LocalisedString name)
        {
            Name = name;
            return this;
        }        
        public LocalisedAuthorBuilder WithUrl(LocalisedString url)
        {
            Url = url;
            return this;
        }
        public LocalisedAuthorBuilder WithIconUrl(LocalisedString iconUrl)
        {
            IconUrl = iconUrl;
            return this;
        }

        object ILocalisable.Localise(ITextResourceCollection textResource)
            => Localise(textResource);
        public EmbedAuthorBuilder Localise(ITextResourceCollection textResource)
            => new EmbedAuthorBuilder
            {
                Name = Name?.Localise(textResource),
                Url = Url?.Localise(textResource),
                IconUrl = IconUrl?.Localise(textResource)
            };

        public static implicit operator LocalisedAuthorBuilder(EmbedAuthorBuilder builder)
            => new LocalisedAuthorBuilder
            {
                IconUrl = (RawString)builder.IconUrl,
                Name = (RawString)builder.Name,
                Url = (RawString)builder.Url
            };

        public static LocalisedAuthorBuilder FromUser(IUser user)
            => new LocalisedAuthorBuilder
            {
                IconUrl = (RawString)user.GetAvatarUrl(),
                Name = (RawString)$"{user.Username}#{user.Discriminator}"
            };

        public static implicit operator LocalisedAuthorBuilder(SocketUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(SocketGuildUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(SocketSelfUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(SocketGroupUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(SocketUnknownUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(SocketWebhookUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(RestUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(RestGuildUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(RestSelfUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(RestGroupUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(RestWebhookUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(RpcUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(RpcGuildUser user)
            => FromUser(user);
        public static implicit operator LocalisedAuthorBuilder(RpcWebhookUser user)
            => FromUser(user);

        #region WithX Overloads
        public LocalisedAuthorBuilder WithRawName(string rawText)
            => WithName(new RawString(rawText));
        public LocalisedAuthorBuilder WithName(string key)
            => WithName(new LocalisedString(key));
        public LocalisedAuthorBuilder WithName(string key, ReplyType replyType)
            => WithName(new LocalisedString(key, replyType));
        public LocalisedAuthorBuilder WithName(string key, params object[] values)
            => WithName(new LocalisedString(key, values));
        public LocalisedAuthorBuilder WithName(string key, ReplyType replyType, params object[] values)
            => WithName(new LocalisedString(key, replyType, values));
        public LocalisedAuthorBuilder WithRawUrl(string rawText)
            => WithUrl(new RawString(rawText));
        public LocalisedAuthorBuilder WithUrl(string key)
            => WithUrl(new LocalisedString(key));
        public LocalisedAuthorBuilder WithUrl(string key, ReplyType replyType)
            => WithUrl(new LocalisedString(key, replyType));
        public LocalisedAuthorBuilder WithUrl(string key, params object[] values)
            => WithUrl(new LocalisedString(key, values));
        public LocalisedAuthorBuilder WithUrl(string key, ReplyType replyType, params object[] values)
            => WithUrl(new LocalisedString(key, replyType, values));
        public LocalisedAuthorBuilder WithRawIconUrl(string rawText)
            => WithIconUrl(new RawString(rawText));
        public LocalisedAuthorBuilder WithIconUrl(string key)
            => WithIconUrl(new LocalisedString(key));
        public LocalisedAuthorBuilder WithIconUrl(string key, ReplyType replyType)
            => WithIconUrl(new LocalisedString(key, replyType));
        public LocalisedAuthorBuilder WithIconUrl(string key, params object[] values)
            => WithIconUrl(new LocalisedString(key, values));
        public LocalisedAuthorBuilder WithIconUrl(string key, ReplyType replyType, params object[] values)
            => WithIconUrl(new LocalisedString(key, replyType, values));
        #endregion
    }

    public class LocalisedFooterBuilder : ILocalisable<EmbedFooterBuilder>
    {
        public LocalisedString Text { get; set; }
        public LocalisedString IconUrl { get; set; }

        public LocalisedFooterBuilder WithText(LocalisedString text)
        {
            Text = text;
            return this;
        }
        public LocalisedFooterBuilder WithIconUrl(LocalisedString iconUrl)
        {
            IconUrl = iconUrl;
            return this;
        }

        object ILocalisable.Localise(ITextResourceCollection textResource)
            => Localise(textResource);
        public EmbedFooterBuilder Localise(ITextResourceCollection textResource)
            => new EmbedFooterBuilder
            {
                Text = Text?.Localise(textResource),
                IconUrl = IconUrl?.Localise(textResource)
            };

        public static implicit operator LocalisedFooterBuilder(EmbedFooterBuilder builder)
            => new LocalisedFooterBuilder
            {
                IconUrl = (RawString)builder.IconUrl,
                Text = (RawString)builder.Text
            };

        #region WithX Overloads
        public LocalisedFooterBuilder WithRawText(string rawText)
            => WithText(new RawString(rawText));
        public LocalisedFooterBuilder WithText(string key)
            => WithText(new LocalisedString(key));
        public LocalisedFooterBuilder WithText(string key, ReplyType replyType)
            => WithText(new LocalisedString(key, replyType));
        public LocalisedFooterBuilder WithText(string key, params object[] values)
            => WithText(new LocalisedString(key, values));
        public LocalisedFooterBuilder WithText(string key, ReplyType replyType, params object[] values)
            => WithText(new LocalisedString(key, replyType, values));
        public LocalisedFooterBuilder WithRawIconUrl(string rawText)
            => WithIconUrl(new RawString(rawText));
        public LocalisedFooterBuilder WithIconUrl(string key)
            => WithIconUrl(new LocalisedString(key));
        public LocalisedFooterBuilder WithIconUrl(string key, ReplyType replyType)
            => WithIconUrl(new LocalisedString(key, replyType));
        public LocalisedFooterBuilder WithIconUrl(string key, params object[] values)
            => WithIconUrl(new LocalisedString(key, values));
        public LocalisedFooterBuilder WithIconUrl(string key, ReplyType replyType, params object[] values)
            => WithIconUrl(new LocalisedString(key, replyType, values));
        #endregion
    }

    public class LocalisedFieldBuilder : ILocalisable<EmbedFieldBuilder>
    {
        public LocalisedString Name { get; set; }
        public LocalisedString Value { get; set; }
        public bool IsInline { get; set; }

        public LocalisedFieldBuilder WithName(LocalisedString name)
        {
            Name = name;
            return this;
        }
        public LocalisedFieldBuilder WithValue(LocalisedString value)
        {
            Value = value;
            return this;
        }
        public LocalisedFieldBuilder WithValue(object value)
        {
            Value = new LocalisedString(value);
            return this;
        }
        public LocalisedFieldBuilder WithIsInline(bool isInline)
        {
            IsInline = isInline;
            return this;
        }

        object ILocalisable.Localise(ITextResourceCollection textResource)
            => Localise(textResource);
        public EmbedFieldBuilder Localise(ITextResourceCollection textResource)
            => new EmbedFieldBuilder
            {
                Name = Name?.Localise(textResource),
                Value = Value?.Localise(textResource),
                IsInline = IsInline
            };

        public static implicit operator LocalisedFieldBuilder(EmbedFieldBuilder builder)
            => new LocalisedFieldBuilder
            {
                IsInline = builder.IsInline,
                Name = (RawString)builder.Name,
                Value = (RawString)builder.Value
            };

        #region WithX Overloads
        public LocalisedFieldBuilder WithRawValue(string rawText)
            => WithValue(new RawString(rawText));
        public LocalisedFieldBuilder WithValue(string key)
            => WithValue(new LocalisedString(key));
        public LocalisedFieldBuilder WithValue(string key, ReplyType replyType)
            => WithValue(new LocalisedString(key, replyType));
        public LocalisedFieldBuilder WithValue(string key, params object[] values)
            => WithValue(new LocalisedString(key, values));
        public LocalisedFieldBuilder WithValue(string key, ReplyType replyType, params object[] values)
            => WithValue(new LocalisedString(key, replyType, values));
        public LocalisedFieldBuilder WithRawName(string rawText)
            => WithName(new RawString(rawText));
        public LocalisedFieldBuilder WithName(string key)
            => WithName(new LocalisedString(key));
        public LocalisedFieldBuilder WithName(string key, ReplyType replyType)
            => WithName(new LocalisedString(key, replyType));
        public LocalisedFieldBuilder WithName(string key, params object[] values)
            => WithName(new LocalisedString(key, values));
        public LocalisedFieldBuilder WithName(string key, ReplyType replyType, params object[] values)
            => WithName(new LocalisedString(key, replyType, values));
        #endregion
    }

    public class RawString : LocalisedString
    {
        public RawString(string text) : base("{0}", text) { }

        public static implicit operator RawString(string text)
            => new RawString(text);
    }

    public class LocalisedString : ILocalisable<string>
    {
        public string Key { get; }
        public ReplyType ReplyType { get; } = ReplyType.None;
        public object[] Values => _values.ToArray();

        private List<object> _values { get; } = new List<object>();

        public LocalisedString(object value) : this("{0}", ReplyType.None, new object[] { value }) { }
        public LocalisedString(string text) : this(text, ReplyType.None, values: null) { }
        public LocalisedString(string key, ReplyType replyType) : this(key, replyType, values: null) { }
        public LocalisedString(string key, params object[] values) : this(key, ReplyType.None, values) { }
        public LocalisedString(string key, ReplyType replyType, params object[] values)
        {
            Key = key;
            ReplyType = replyType;
            _values.AddRange(values ?? new object[0]);
        }

        public LocalisedString AddValue(object value)
        {
            _values.Add(value);
            return this;
        }

        object ILocalisable.Localise(ITextResourceCollection textResource)
            => Localise(textResource);
        public string Localise(ITextResourceCollection textResource)
        {
            if (Key == null) return null;
            var prepped = _values.Select(v => v is ILocalisable ls ? ls.Localise(textResource) : v).ToArray();
            return textResource.Format(Key, ReplyType, prepped);
        }

        public override string ToString()
            => Key;
        
        private static LocalisedString Build(string text, ReplyType replyType = ReplyType.None, object[] values = null)
        {
            if (text == null) return null;            

            return new LocalisedString(text, replyType, values ?? new object[0]);
        }

        public static explicit operator LocalisedString(string text)
            => Build(text);
        public static implicit operator LocalisedString((string Text, object Value1) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1 });
        public static implicit operator LocalisedString((string Text, object Value1, object Value2) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2 });
        public static implicit operator LocalisedString((string Text, object Value1, object Value2, object Value3) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3 });
        public static implicit operator LocalisedString((string Text, object Value1, object Value2, object Value3, object Value4) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4 });
        public static implicit operator LocalisedString((string Text, object Value1, object Value2, object Value3, object Value4, object Value5) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4, tuple.Value5 });
        public static implicit operator LocalisedString((string Text, object Value1, object Value2, object, object Value3, object Value4, object Value5, object Value6) tuple)
            => Build(tuple.Text, values: new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4, tuple.Value5, tuple.Value6});
        public static implicit operator LocalisedString((string Key, ReplyType ReplyType) tuple)
            => Build(tuple.Key, tuple.ReplyType);
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1 });
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1, object Value2) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1, tuple.Value2 });
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1, object Value2, object Value3) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1, tuple.Value2, tuple.Value3 });
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1, object Value2, object Value3, object Value4) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4 });
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1, object Value2, object Value3, object Value4, object Value5) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4, tuple.Value5 });
        public static implicit operator LocalisedString((string Text, ReplyType ReplyType, object Value1, object Value2, object, object Value3, object Value4, object Value5, object Value6) tuple)
            => Build(tuple.Text, tuple.ReplyType, new object[] { tuple.Value1, tuple.Value2, tuple.Value3, tuple.Value4, tuple.Value5, tuple.Value6 });
        public static implicit operator LocalisedString((string Key, object[] Values) tuple)
            => Build(tuple.Key, values: tuple.Values);
        public static implicit operator LocalisedString((string Key, ReplyType ReplyType, object[] Values) tuple)
            => Build(tuple.Key, tuple.ReplyType, tuple.Values);
    }
}
