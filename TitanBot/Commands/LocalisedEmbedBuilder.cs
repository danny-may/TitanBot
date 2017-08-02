using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Formatting;
using TitanBot.Replying;
using TitanBot.Util;

namespace TitanBot.Commands
{
    public class LocalisedEmbedBuilder
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
        public LocalisedEmbedBuilder WithAuthor(LocalisedString name, LocalisedString iconUrl = null, LocalisedString url = null)
        {
            var author = new LocalisedAuthorBuilder
            {
                Name = name,
                IconUrl = iconUrl,
                Url = url
            };
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
        public LocalisedEmbedBuilder WithFooter(LocalisedString text, LocalisedString iconUrl = null)
        {
            var footer = new LocalisedFooterBuilder
            {
                Text = text,
                IconUrl = iconUrl
            };
            Footer = footer;
            return this;
        }

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
            AddField(field);
            return this;
        }
        public LocalisedEmbedBuilder AddField(LocalisedString title, LocalisedString text, bool inline = false)
        {
            var field = new LocalisedFieldBuilder
            {
                Name = title,
                Value = text,
                IsInline = inline
            };
            _fields.Add(field);
            return this;
        }
        public LocalisedEmbedBuilder AddInlineField(LocalisedString title, LocalisedString text)
            => AddField(title, text, true);

        public EmbedBuilder Localise(ITextResourceCollection textResource)
            => new EmbedBuilder
            {
                Author = Author?.Localise(textResource),
                Color = Color,
                Description = Description?.ToString(textResource),
                Fields = Fields.Select(f => f?.Localise(textResource)).Where(f => f != null).ToList(),
                Footer = Footer?.Localise(textResource),
                ImageUrl = ImageUrl?.ToString(textResource),
                ThumbnailUrl = ThumbnailUrl?.ToString(textResource),
                Timestamp = Timestamp,
                Title = Title?.ToString(textResource),
                Url = Url?.ToString(textResource)
            };
    }

    public class LocalisedAuthorBuilder
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

        public EmbedAuthorBuilder Localise(ITextResourceCollection textResource)
            => new EmbedAuthorBuilder
            {
                Name = Name?.ToString(textResource),
                Url = Url?.ToString(textResource),
                IconUrl = IconUrl?.ToString(textResource)
            };
    }

    public class LocalisedFooterBuilder
    {
        public LocalisedString Text { get; set; }
        public LocalisedString IconUrl { get; set; }
        
        public LocalisedFooterBuilder WithText(string text)
        {
            Text = text;
            return this;
        }
        public LocalisedFooterBuilder WithIconUrl(string iconUrl)
        {
            IconUrl = iconUrl;
            return this;
        }

        public EmbedFooterBuilder Localise(ITextResourceCollection textResource)
            => new EmbedFooterBuilder
            {
                Text = Text?.ToString(textResource),
                IconUrl = IconUrl?.ToString(textResource)
            };
    }

    public class LocalisedFieldBuilder
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

        public EmbedFieldBuilder Localise(ITextResourceCollection textResource)
            => new EmbedFieldBuilder
            {
                Name = Name?.ToString(textResource),
                Value = Value?.ToString(textResource),
                IsInline = IsInline
            };
    }

    public class LocalisedString
    {
        public string Key { get; }
        public ReplyType ReplyType { get; } = ReplyType.None;
        public object[] Values => _values.ToArray();

        private List<object> _values { get; } = new List<object>();

        public LocalisedString(object value) : this("_{0}", ReplyType.None, new object[] { value }) { }
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

        public string ToString(ITextResourceCollection textResource)
        {
            if (Key == null) return null;
            var prepped = _values.Select(v => v is LocalisedString ls ? ls.ToString(textResource) : v).ToArray();
            return textResource.Format(Key, ReplyType, prepped);
        }

        public override string ToString()
        {
            return Key;
        }
        
        private static LocalisedString Build(string text, ReplyType replyType = ReplyType.None, object[] values = null)
        {
            if (text == null) return null;

            return new LocalisedString(text, replyType, values ?? new object[0]);
        }

        //Done like this because I cant implicit convert from object because c# is dumb :(
        public static implicit operator LocalisedString(int value)
            => new LocalisedString(value);
        public static implicit operator LocalisedString(double value)
            => new LocalisedString(value);
        public static implicit operator LocalisedString(long value)
            => new LocalisedString(value);
        public static implicit operator LocalisedString(uint value)
            => new LocalisedString(value);
        public static implicit operator LocalisedString(ulong value)
            => new LocalisedString(value);
        public static implicit operator LocalisedString(DateTime value)
            => new LocalisedString(value);
        public static implicit operator LocalisedString(TimeSpan value)
            => new LocalisedString(value);
        public static implicit operator LocalisedString(bool value)
            => new LocalisedString(value);

        public static implicit operator LocalisedString(string text)
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
