using Discord;
using System;
using System.Threading.Tasks;
using TitanBot.Contexts;
using TitanBot.Formatting;
using TitanBot.Scheduling;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [RequireOwner]
    class TestCommand : Command
    {
        [Call("Flag")]
        async Task TestFlagsAsync([CallFlag('a', "aflag", "Bool flag")]bool a = false, [CallFlag('b', "Bool flag")]bool b = true, [CallFlag('c', "cflag", "String flag")]string c = "Not provided")
        {
            await ReplyAsync(new EmbedBuilder().AddField("a", a)
                                               .AddField("b", b)
                                               .AddField("c", c));
        }

        [Call("Scheduler")]
        async Task TestSchedulerAsync(int delay = 1)
        {
            for (int i = 0; i < 50; i++)
                Scheduler.Queue<SchedulerTestHandler>(BotUser.Id, null, DateTime.Now, new TimeSpan(0, 0, delay), DateTime.Now.AddSeconds(33));

            await ReplyAsync((RawString)"Queued uccessfully");
        }

        public class SchedulerTestHandler : ISchedulerCallback
        {
            public void Complete(ISchedulerContext context, bool wasCancelled)
                => Handle(null);

            public void Handle(ISchedulerContext context)
            {
                var endAt = DateTime.Now.AddMilliseconds(200);
                while (DateTime.Now < endAt) { }
            }
        }

        [Call("Embed")]
        async Task TestEmbedAsync()
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
