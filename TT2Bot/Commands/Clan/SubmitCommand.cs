using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Util;
using TT2Bot.Models;
using TT2Bot.Models.Database;

namespace TitanBot2.Commands.Clan
{
    [Description("Allows you to submit a bug, suggestion or question to GameHive!")]
    [RequireGuild(169160979744161793)]
    class SubmitCommand : Command
    {
        private TT2GlobalSettings TT2Settings => SettingsManager.GetCustomGlobal<TT2GlobalSettings>();
        private IMessageChannel SubmissionChannel => Client.GetChannel(TT2Settings.GHFeedbackChannel) as IMessageChannel;

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

            IUser submitter = Client.GetUser(submission.Submitter);
            IUser replier = null;
            if (submission.Answerer != null)
                replier = Client.GetUser(submission.Answerer.Value);

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

        async Task SubmitAsync(string title, TT2Submission.SubmissionType type, string description, Uri image, Uri reddit)
        {
            var canSubmit = (await Database.FindById<PlayerData>(Author.Id))?.CanGHSubmit ?? true;
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

            if (string.IsNullOrWhiteSpace(description))
            {
                await ReplyAsync("Please make sure you provide a description using the `-d` flag!");
                return;
            }

            

            var urlString = Message.Attachments.FirstOrDefault(a => Regex.IsMatch(a.Url, TT2Settings.ImageRegex))?.Url;
            Uri imageUrl = null;
            if (urlString != null)
                if (image != null)
                {
                    await ReplyAsync("You cannot specify an image flag if you attach an image!", ReplyType.Error);
                    return;
                }
                else
                    imageUrl = new Uri(urlString);
            else if (image != null && !Regex.IsMatch(image.AbsoluteUri, TT2Settings.ImageRegex))
            {
                await ReplyAsync("The image you supplied is not a valid image!", ReplyType.Error);
                return;
            }
            else if (image != null)
                imageUrl = image;


            Uri redditUrl = null;
            if (reddit != null && !Regex.IsMatch(reddit.AbsoluteUri, @"^https?:\/\/(www.)?redd(.it|it.com)\/.*$"))
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
                Submitter = Author.Id,
                Type = type,
                SubmissionTime = DateTime.Now
            };

            await Database.Insert(submission);

            var message = await ReplyAsync(SubmissionChannel, "", embed: GetSubmissionMessage(submission).Build());

            submission.Message = message.Id;
            await Database.Upsert(submission);

            await ReplyAsync($"Your {type} has been successfully sent!", ReplyType.Success);
        }

        async Task BlockUserAsync(IUser target, bool block)
        {
            var user = await Database.FindById<PlayerData>(target.Id) ?? new PlayerData
            {
                Id = target.Id
            };

            user.CanGHSubmit = !block;

            await Database.Upsert(user);

            await ReplyAsync("Successfully " + (block ? "blocked" : "unblocked") + " that user!", ReplyType.Success);
        }

        [Call("Bug")]
        [Usage("Submits a bug to the GH team")]
        Task SubmitBugAsync([Dense]string title,
            [CallFlag('d', "description", "Describe the submission")]string description = null,
            [CallFlag('i', "image", "Links an image to the submission")]Uri image = null,
            [CallFlag('r', "reddit", "Links a reddit url to the submission")]Uri reddit = null)
            => SubmitAsync(title, TT2Submission.SubmissionType.Bug, description, image, reddit);

        [Call("Suggestion")]
        [Usage("Submits a suggestion to the GH team")]
        Task SubmitFeatureAsync([Dense]string title,
            [CallFlag('d', "description", "Describe the submission")]string description = null,
            [CallFlag('i', "image", "Links an image to the submission")]Uri image = null,
            [CallFlag('r', "reddit", "Links a reddit url to the submission")]Uri reddit = null)
            => SubmitAsync(title, TT2Submission.SubmissionType.Suggestion, description, image, reddit);

        [Call("Question")]
        [Usage("Submits a question to the GH team")]
        Task SubmitQuestionAsync([Dense]string title,
            [CallFlag('d', "description", "Describe the submission")]string description = null,
            [CallFlag('i', "image", "Links an image to the submission")]Uri image = null,
            [CallFlag('r', "reddit", "Links a reddit url to the submission")]Uri reddit = null)
            => SubmitAsync(title, TT2Submission.SubmissionType.Question, description, image, reddit);

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
        async Task ReplySubmissionAsync(ulong id, [Dense]string reply,
            [CallFlag('q', "quiet", "Specifies that there should be no DM sent to the submitter")]bool quiet = false)
        {
            var submission = await Database.FindById<TT2Submission>(id);
            if (submission == null)
            {
                await ReplyAsync("There is no submission by that ID", ReplyType.Error);
                return;
            }

            var alreadyReplied = submission.Response != null;

            submission.ReplyTime = DateTime.Now;
            submission.Response = reply;
            submission.Answerer = Author.Id;

            await Database.Upsert(submission);

            if (submission.Message != null && await SubmissionChannel.GetMessageAsync(submission.Message.Value) is IUserMessage message)
                await message.ModifySafeAsync(m => m.Embed = GetSubmissionMessage(submission).Build());

            var submitter = Client.GetUser(submission.Submitter);
            if (submitter != null && !alreadyReplied && !quiet)
                await ReplyAsync(submitter, $"Your recent {submission.Type} titled `{submission.Title}` has just been replied to:\n\n{reply}\n - {Author}");

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

            var unanswered = await Database.Find<TT2Submission>(s => s.Response == null);
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
                var user = Client.GetUser(submission.Submitter);
                table.Add(new string[]{
                    submission.Id.ToString(),
                    submission.Title.Substring(0, 50),
                    $"[{submission.Type}]",
                    user != null ? $"{user} ({user.Id})" : $"UNKNOWN USER ({submission.Id})"
                });
            }

            await ReplyAsync($"```css\n{table.ToArray().Tableify()}```");
        }

        [Call("Show")]
        [Usage("Pulls the text for any submission")]
        [DefaultPermission(8)]
        async Task ShowAsync(ulong id)
        {
            var submission = await Database.FindById<TT2Submission>(id);
            if (submission == null)
                await ReplyAsync("I could not find that submission", ReplyType.Error);
            else
            {
                await ReplyAsync("Ill DM you the submission!", ReplyType.Success);
                await ReplyAsync(Author, "", embed: GetSubmissionMessage(submission));
            }
        }
    }
}
