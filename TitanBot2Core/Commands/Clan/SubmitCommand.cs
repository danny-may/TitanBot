using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.Database.Tables;

namespace TitanBot2.Commands.Clan
{
    [Description("Allows you to submit a bug, suggestion or question to GameHive!")]
    [RequireGuild(169160979744161793)]
    class SubmitCommand : Command
    {
        private IMessageChannel SubmissionChannel => Context.Client.GetChannel(Configuration.Instance.SubmissionChannel) as IMessageChannel;

        EmbedBuilder GetSubmissionMessage(TT2Submission submission)
        {
            var color = System.Drawing.Color.Pink.ToDiscord();
            switch (submission.Type)
            {
                case TT2Submission.SubmissionType.Bug:
                    color = System.Drawing.Color.Red.ToDiscord();
                    break;
                case TT2Submission.SubmissionType.Question:
                    color = System.Drawing.Color.SkyBlue.ToDiscord();
                    break;
                case TT2Submission.SubmissionType.Suggestion:
                    color = System.Drawing.Color.Green.ToDiscord();
                    break;
            }

            IUser submitter = Context.Client.GetUser(submission.Submitter);
            IUser replier = null;
            if (submission.Answerer != null)
                replier = Context.Client.GetUser(submission.Answerer.Value);

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = submitter?.GetAvatarUrl(),
                    Name = $"{submitter?.ToString() ?? "Unknown User"} ({submission.Submitter})"
                },
                Color = color,
                Title = $"{submission.Type} #{submission.Id}: {submission.Title}",
                Description = submission.Description,
                Timestamp = submission.SubmissionTime
            };

            if (submission.Reddit != null)
                builder.AddField("Reddit link", submission.Reddit);
            if (submission.ImageUrl != null)
                builder.AddField("Image", submission.ImageUrl);
            if (string .IsNullOrWhiteSpace(submission.Response))
                builder.AddField("GH reply", "No reply yet!");
            else
                builder.AddField("GH reply", submission.Response);

            if (replier != null)
                builder.WithFooter(new EmbedFooterBuilder
                {
                    IconUrl = replier.GetAvatarUrl(),
                    Text = $"{replier} | Replied {submission.ReplyTime}"
                });

            return builder;
        }

        async Task SubmitAsync(string title, TT2Submission.SubmissionType type)
        {
            var canSubmit = (await Context.Database.Users.Find(Context.User.Id))?.CanSubmit ?? true;
            if (!canSubmit)
            {
                await ReplyAsync("You have been blocked from using this command. Please contact one of the admins on the /r/TapTitans2 discord if you think this is a mistake.", ReplyType.Error);
                return;
            }

            if (SubmissionChannel == null)
            {
                await ReplyAsync($"I could not locate where to send the {type}! Please try again later.", ReplyType.Error);
                return;
            }

            if (!Flags.TryGet("d", out string description))
            {
                await ReplyAsync("Please make sure you provide a description using the `-d <description` flag!");
                return;
            }

            var imageExtensions = new string[] { ".png", ".jpg", ".gif", ".svg" };

            var urlString = Context.Message.Attachments.FirstOrDefault(a => a.Url.EndsWithAny(imageExtensions))?.Url;
            Uri imageUrl = null;
            if (urlString != null)
                if (Flags.Has("i"))
                {
                    await ReplyAsync("You cannot specify an image flag if you attach an image!", ReplyType.Error);
                    return;
                }
                else
                    imageUrl = new Uri(urlString);
            else if (Flags.Has("i"))
            {
                if (!Flags.TryGet("i", out Uri flagUrl) || !flagUrl.AbsoluteUri.EndsWithAny(imageExtensions))
                {
                    await ReplyAsync("The image you supplied is not a valid image!", ReplyType.Error);
                    return;
                }
                imageUrl = flagUrl;
            }


            Uri redditUrl = null;
            if (Flags.Has("r") && (!Flags.TryGet("r", out redditUrl) || !Regex.IsMatch(redditUrl.AbsoluteUri, @"^https?:\/\/(www.)?redd(.it|it.com)\/.*$")))
            {
                await ReplyAsync("The URL you gave was not a valid reddit URL", ReplyType.Error);
                return;
            }

            var submission = new TT2Submission
            {
                Description = description,
                Title = title,
                ImageUrl = imageUrl,
                Reddit = redditUrl,
                Submitter = Context.User.Id,
                Type = type,
                SubmissionTime = DateTime.Now
            };

            await Context.Database.Submissions.Insert(submission);

            var message = await TrySend(SubmissionChannel.Id, "", embed: GetSubmissionMessage(submission).Build());

            submission.Message = message.Id;
            await Context.Database.Submissions.Upsert(submission);

            await ReplyAsync($"Your {type} has been successfully sent!", ReplyType.Success);
        }

        async Task BlockUserAsync(IUser target, bool block)
        {
            var user = await Context.Database.Users.Find(target.Id) ?? new User
            {
                DiscordId = target.Id,
            };

            user.CanSubmit = !block;

            await Context.Database.Users.Upsert(user);

            await ReplyAsync("Successfully " + (block ? "blocked" : "unblocked") + " that user!", ReplyType.Success);
        }

        [Call("Bug")]
        [Usage("Submits a bug to the GH team")]
        [CallFlag(typeof(string), "d", "description", "Describe the submission")]
        [CallFlag(typeof(Uri), "i", "image", "Links an image to the submission")]
        [CallFlag(typeof(Uri), "r", "reddit", "Links a reddit url to the submission")]
        Task SubmitBugAsync([Dense]string title)
            => SubmitAsync(title, TT2Submission.SubmissionType.Bug);

        [Call("Suggestion")]
        [Usage("Submits a suggestion to the GH team")]
        [CallFlag(typeof(string), "d", "description", "Describe the submission")]
        [CallFlag(typeof(Uri), "i", "image", "Links an image to the submission")]
        [CallFlag(typeof(Uri), "r", "reddit", "Links a reddit url to the submission")]
        Task SubmitFeatureAsync([Dense]string title)
            => SubmitAsync(title, TT2Submission.SubmissionType.Suggestion);

        [Call("Question")]
        [Usage("Submits a question to the GH team")]
        [CallFlag(typeof(string), "d", "description", "Describe the submission")]
        [CallFlag(typeof(Uri), "i", "image", "Links an image to the submission")]
        [CallFlag(typeof(Uri), "r", "reddit", "Links a reddit url to the submission")]
        Task SubmitQuestionAsync([Dense]string title)
            => SubmitAsync(title, TT2Submission.SubmissionType.Question);

        [Call("Block")]
        [Usage("Blocks a user from being able to use the submit command")]
        [DefaultPermission(8)]
        Task BlockUserAsync([Dense]IUser user)
            => BlockUserAsync(user, true);

        [Call("Unblock")]
        [Usage("Unblocks a user, allowing them to use the submit command")]
        [DefaultPermission(8)]
        Task UnblockUserAsync([Dense]IUser user)
            => BlockUserAsync(user, false);

        [Call("Reply")]
        [Usage("Replys to a given submission")]
        [DefaultPermission(8)]
        [CallFlag("q", "quiet", "Specifies that there should be no DM sent to the submitter")]
        async Task ReplySubmissionAsync(int id, [Dense]string reply)
        {
            var submission = await Context.Database.Submissions.Find(id);
            if (submission == null)
            {
                await ReplyAsync("There is no submission by that ID", ReplyType.Error);
                return;
            }

            var alreadyReplied = submission.Response != null;

            submission.ReplyTime = DateTime.Now;
            submission.Response = reply;
            submission.Answerer = Context.User.Id;

            await Context.Database.Submissions.Upsert(submission);

            if (submission.Message != null && await SubmissionChannel.GetMessageAsync(submission.Message.Value) is IUserMessage message)
                await message.ModifySafeAsync(m => m.Embed = GetSubmissionMessage(submission).Build());

            var submitter = Context.Client.GetUser(submission.Submitter);
            if (submitter != null && !alreadyReplied && !Flags.Has("q"))
                await TrySend(submitter.Id, $"Your recent {submission.Type} titled `{submission.Title}` has just been replied to:\n\n{reply}\n - {Context.User}");

            await ReplyAsync("Reply has been accepted!", ReplyType.Success);
        }

        [Call("List")]
        [Usage("Lists all unanswered submissions")]
        [DefaultPermission(8)]
        async Task ListAsync(TT2Submission.SubmissionType[] type = null)
        {
            if (type == null)
                type = new TT2Submission.SubmissionType[]
                {
                    TT2Submission.SubmissionType.Bug,
                    TT2Submission.SubmissionType.Question,
                    TT2Submission.SubmissionType.Suggestion
                };

            var unanswered = await Context.Database.Submissions.Get(s => s.Response == null);
            unanswered = unanswered.Where(s => type.Contains(s.Type));

            if (unanswered.Count() == 0)
            {
                await ReplyAsync("There are no unanswered submissions!", ReplyType.Info);
                return;
            }

            var table = new List<string[]>();
            table.Add(new string[]
            {
                "id",
                "Title",
                "Type",
                "Submitter"
            });

            foreach (var submission in unanswered)
            {
                var user = Context.Client.GetUser(submission.Submitter);
                table.Add(new string[]{
                    submission.Id.ToString(),
                    submission.Title.MaxLength(50),
                    $"[{submission.Type}]",
                    user != null ? $"{user} ({user.Id})" : $"UNKNOWN USER ({submission.Id})"
                });
            }

            await ReplyAsync($"```css\n{table.ToArray().Tableify()}```");
        }

        [Call("Show")]
        [Usage("Pulls the text for any submission")]
        [DefaultPermission(8)]
        async Task ShowAsync(int id)
        {
            var submission = await Context.Database.Submissions.Find(id);
            if (submission == null)
                await ReplyAsync("I could not find that submission", ReplyType.Error);
            else
            {
                await ReplyAsync("Ill DM you the submission!", ReplyType.Success);
                await TrySend(Context.User.Id, "", embed: GetSubmissionMessage(submission));
            }
        }
    }
}
