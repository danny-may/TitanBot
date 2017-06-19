using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Commands.Models
{
    class SudoMessage : IUserMessage
    {
        public SudoMessage(IUserMessage baseMessage)
        {
            Reactions = baseMessage.Reactions.ToDictionary(r => r.Key, r => r.Value);
            Type = baseMessage.Type;
            Source = baseMessage.Source;
            IsTTS = baseMessage.IsTTS;
            IsPinned = baseMessage.IsPinned;
            Content = baseMessage.Content;
            Timestamp = baseMessage.Timestamp;
            EditedTimestamp = baseMessage.EditedTimestamp;
            Channel = baseMessage.Channel;
            Author = baseMessage.Author;
            Attachments = baseMessage.Attachments;
            Embeds = baseMessage.Embeds;
            Tags = baseMessage.Tags;
            MentionedChannelIds = baseMessage.MentionedChannelIds;
            MentionedRoleIds = baseMessage.MentionedRoleIds;
            MentionedUserIds = baseMessage.MentionedUserIds;
            CreatedAt = baseMessage.CreatedAt;
            Id = baseMessage.Id;
        }

        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions { get; set; }

        public MessageType Type { get; set; }

        public MessageSource Source { get; set; }

        public bool IsTTS { get; set; }

        public bool IsPinned { get; set; }

        public string Content { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DateTimeOffset? EditedTimestamp { get; set; }

        public IMessageChannel Channel { get; set; }

        public IUser Author { get; set; }

        public IReadOnlyCollection<IAttachment> Attachments { get; set; }

        public IReadOnlyCollection<IEmbed> Embeds { get; set; }

        public IReadOnlyCollection<ITag> Tags { get; set; }

        public IReadOnlyCollection<ulong> MentionedChannelIds { get; set; }

        public IReadOnlyCollection<ulong> MentionedRoleIds { get; set; }

        public IReadOnlyCollection<ulong> MentionedUserIds { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ulong Id { get; set; }

        public Task AddReactionAsync(IEmote emote, RequestOptions options = null)
        {
            ((IDictionary<IEmote, ReactionMetadata>)Reactions).Add(emote, new ReactionMetadata());
            return Task.CompletedTask;
        }

        public Task DeleteAsync(RequestOptions options = null)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(string emoji, int limit = 100, ulong? afterUserId = default(ulong?), RequestOptions options = null)
        {
            return Task.FromResult((IReadOnlyCollection<IUser>)new List<IUser>());
        }

        public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            var props = new MessageProperties { Content = Content, Embed = Optional.Create(Embeds.FirstOrDefault() as Embed) };
            func(props);
            Content = props.Content.Value;
            Embeds = props.Embed.IsSpecified ? new List<IEmbed> { props.Embed.Value } : new List<IEmbed>();
            return Task.CompletedTask;
        }

        public Task PinAsync(RequestOptions options = null)
        {
            IsPinned = true;
            return Task.CompletedTask;
        }

        public Task RemoveAllReactionsAsync(RequestOptions options = null)
        {
            Reactions = new Dictionary<IEmote, ReactionMetadata>();
            return Task.CompletedTask;
        }

        public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null)
        {
            return Task.CompletedTask;
        }

        public string Resolve(TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name, TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
        {
            throw new NotImplementedException();
        }

        public Task UnpinAsync(RequestOptions options = null)
        {
            IsPinned = false;
            return Task.CompletedTask;
        }
    }
}
