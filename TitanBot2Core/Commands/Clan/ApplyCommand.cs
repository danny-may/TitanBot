using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.CommandService.Models;
using TitanBot2.Services.Database.Tables;

namespace TitanBot2.Commands.Clan
{
    [Description("Allows a user to register their interest in joining the clan")]
    [DefaultPermission(8)]
    [Alias("R", "Reg", "Register")]
    class ApplyCommand : Command
    {
        [Call]
        [Usage("Creates your application for this clan")]
        [CallFlag("g", "global", "Specifies that the application is a global application.")]
        [CallFlag(typeof(Uri[]), "i", "images", "Specifies a list of images to use with your application")]
        [DefaultPermission(0)]
        [RequireContext(ContextType.Guild)]
        async Task RegisterGuildAsync(int maxStage, [Dense]string message)
        {
            if (Flags.Has("g"))
                await RegisterAsync(maxStage, message, null);
            else
                await RegisterAsync(maxStage, message, Context.Guild.Id);
        }

        [Call]
        [Usage("Creates a global application")]
        [CallFlag(typeof(Uri[]), "i", "images", "Specifies a list of images to use with your registration")]
        [RequireContext(ContextType.DM|ContextType.Group)]
        Task RegisterGlobalAsync(int maxStage, [Dense]string message)
            => RegisterAsync(maxStage, message, null);

        async Task RegisterAsync(int maxStage, string message, ulong? guildId)
        {
            Flags.TryGet("i", out Uri[] images);
            var current = await Context.Database.Registrations.GetForUserOnGuild(Context.User.Id, guildId);
            var isNew = current == null;
            current = current ?? new Registration
            {
                UserId = Context.User.Id,
                GuildId = guildId,
            };

            if (images != null)
                current.Images = images;
            current.MaxStage = maxStage;
            current.Message = message;
            current.EditTime = DateTime.Now;

            await Context.Database.Registrations.Upsert(current);

            if (isNew && guildId == null)
                await ReplyAsync("Your global application has been successful. Recruiters from any TT2 guild will be able to see your application and might choose to recruit you.", ReplyType.Success);
            else if (isNew)
                await ReplyAsync("Your application has been successful. The clan recruiter will review your application and potentially get back to you.");
            else
                await ReplyAsync("Your global application has been successfully updated", ReplyType.Success);
        }

        [Call("View")]
        [Usage("Views your registration for this guild")]
        [RequireContext(ContextType.Guild)]
        [DefaultPermission(0)]
        Task ViewGuildRegistrationAsync()
            => ViewRegistrationAsync(Context.Guild.Id, Context.User);

        [Call("View")]
        [Usage("Views your global registration")]
        [RequireContext(ContextType.DM|ContextType.Group)]
        Task ViewGlobalRegistrationAsync()
            => ViewRegistrationAsync(null, Context.User);

        [Call("Show")]
        [Usage("Shows the registration for the given user")]
        [RequireContext(ContextType.Guild)]
        Task ViewGuildRegistrationAsync(SocketUser user)
            => ViewRegistrationAsync(Context.Guild.Id, user);

        async Task ViewRegistrationAsync(ulong? guildId, IUser user)
        {
            var current = await Context.Database.Registrations.GetForUserOnGuild(user?.Id ?? Context.User.Id, guildId) ??
                          await Context.Database.Registrations.GetForUserOnGuild(user?.Id ?? Context.User.Id, null);

            if (current == null)
            {
                if (user.Id == Context.User.Id)
                    await ReplyAsync("You have not registered yet!", ReplyType.Error);
                else
                    await ReplyAsync("That user does not have a registration here yet!", ReplyType.Error);
                return;
            }

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = user.GetAvatarUrl(),
                    Name = $"{user} ({user.Id})"
                },
                Color = System.Drawing.Color.SkyBlue.ToDiscord(),
                Description = current.Message,
                Title = "Application",
                Footer = new EmbedFooterBuilder
                {
                    Text = $"{(current.GuildId == null ? "Global" : "Local")} application | Applied {current.ApplyTime} | Updated {current.EditTime}"
                }
            }.AddField("Images", string.Join("\n", current.Images?.Select(i => i.AbsoluteUri) ?? new string[] { "None" }));

            await ReplyAsync("", embed: builder);
        }

        [Call("Cancel")]
        [Usage("Cancels your registration for this guild")]
        [RequireContext(ContextType.Guild)]
        [DefaultPermission(0)]
        Task RemoveGuildRegistrationAsync()
            => RemoveRegistrationAsync(Context.Guild.Id, Context.User);

        [Call("Cancel")]
        [Usage("Cancels your global registration")]
        [RequireContext(ContextType.DM | ContextType.Group)]
        Task RemoveGlobalRegistrationAsync()
            => RemoveRegistrationAsync(null, Context.User);

        [Call("Remove")]
        [Usage("Removes the registration for the given user")]
        [RequireContext(ContextType.Guild)]
        Task RemoveGuildRegistrationAsync(SocketUser user)
            => RemoveRegistrationAsync(Context.Guild.Id, user);

        async Task RemoveRegistrationAsync(ulong? guildID, IUser user)
        {
            await Context.Database.Registrations.RemoveRegistration(user.Id, guildID);

            if (user.Id == Context.User.Id)
                await ReplyAsync($"You have successfully removed your {(guildID == null ? "global " : "")}application{(guildID != null ? " for this guild" : "")}", ReplyType.Success);
            else
                await ReplyAsync($"You have successfully removed the application by {user.Username} for this guild");
        }

        [Call("List")]
        [Usage("Lists all applications for this guild")]
        [RequireContext(ContextType.Guild)]
        [CallFlag("g", "global", "Specifies that the application is a global application.")]
        async Task ListRegistrationsAsync(int? start = null, int? end = null)
        {
            var from = start ?? 0;
            var to = end ?? from + 20;
            IntExtensions.EnsureOrder(ref from, ref to);
            to.Clamp(from, from + 30);

            var includeGlobal = Flags.Has("g");

            var applications = await Context.Database.Registrations.GetForGuild(Context.Guild.Id);
            applications = applications.Where(a => includeGlobal || a.GuildId != null)
                                       .OrderByDescending(a => a.MaxStage)
                                       .ThenByDescending(a => a.Images?.Count() ?? 0)
                                       .ThenBy(a => a.ApplyTime)
                                       .FirstFor(a => a.UserId)
                                       .Skip(from);

            var table = new List<string[]> { };
            table.Add(new string[]
            {
                "#",
                "User",
                "MS",
                "#Images",
                "Global",
                "Last edit"
            });
            var pos = from + 1;
            foreach (var app in applications)
            {
                var user = Context.Client.GetUser(app.UserId);
                if (user == null)
                    continue;
                table.Add(new string[]{
                    pos++.ToString(),
                    $"{user} ({user.Id})",
                    app.MaxStage.ToString(),
                    (app.Images?.Length ?? 0).ToString(),
                    app.GuildId != null ? "False" : "True",
                    (DateTime.Now - app.EditTime).Days + " day(s) ago"
                });
                if (table.Count == to - from)
                    break;
            }

            var text = table.ToArray().Tableify();

            if (table.Count == 1)
                await ReplyAsync("You have no registered users!", ReplyType.Error);
            else
                await ReplyAsync($"```{text}```");
        }

        [Call("Clear")]
        [Usage("Completely clears your guilds application list")]
        [RequireContext(ContextType.Guild)]
        async Task ClearRegistrationsAsync()
        {
            await Context.Database.Registrations.RemoveRegistrations(Context.Guild.Id);
            await ReplyAsync("Your guilds registrations have been wiped", ReplyType.Success);
        }
    }
}
