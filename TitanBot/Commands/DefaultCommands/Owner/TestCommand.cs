using Discord;
using System;
using System.Threading.Tasks;
using TitanBot.Formatting;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [RequireOwner]
    class TestCommand : Command
    {
        [Call]
        async Task TestAsync()
        {
            
            var builder = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = "http://i.imgur.com/9TVNFSA.mp4",
                    Name = "AuthorName",
                    Url = "http://www.imgur.com/9TVNFSA"
                },
                Color = System.Drawing.Color.Red.ToDiscord(),
                Description = "Description",
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = "http://i.imgur.com/WZWfWHY.jpg",
                    Text = "FooterText"
                },
                ImageUrl = "http://i.imgur.com/bGN0kiO.mp4",
                ThumbnailUrl = "http://i.imgur.com/bGN0kiO.mp4",
                Timestamp = DateTime.MaxValue,
                Title = "Title",
                Url = "http://i.imgur.com/atWR9PM.mp4"
            };
            for (int i = 0; i < 20; i++)
                builder.AddField($"{i}^2 = ", Math.Pow(i, 2));

            var embedable = (Embedable)builder;
            await Reply(Channel).WithEmbedable(embedable).SendAsync();
        }
    }
}
