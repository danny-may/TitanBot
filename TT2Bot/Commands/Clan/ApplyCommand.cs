using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBotBase.Commands;
using TitanBotBase.Util;
using TT2Bot.Models;

namespace TT2Bot.Commands.Clan
{
    [Description("Allows a user to register their interest in joining the clan")]
    [DefaultPermission(8)]
    [Alias("R", "Reg", "Register")]
    class ApplyCommand : Command
    {
        private TT2GlobalSettings TT2GlobalSettings => GlobalSettings.GetCustom<TT2GlobalSettings>();

        [Call]
        [Usage("Creates your application for this clan")]
        [DefaultPermission(0)]
        [RequireContext(ContextType.Guild)]
        private async Task RegisterGuildAsync(int maxStage, [Dense]string message,
            [CallFlag('g', "global", "Specifies that the application is a global application.")]bool isGlobal = false,
            [CallFlag('i', "images", "Specifies a list of images to use with your application")]Uri[] images = null,
            [CallFlag('r', "relics", "Specifies how many relics you have earned")]double relics = -1,
            [CallFlag('a', "attacks", "Specifies how many attacks you aim to do per week")]int attacks = -1,
            [CallFlag('t', "taps", "Specifies how many taps you average per CQ")]int taps = -1)
        {
            if (isGlobal)
                await RegisterAsync(maxStage, message, null, images, relics, attacks, taps);
            else
                await RegisterAsync(maxStage, message, Guild.Id, images, relics, attacks, taps);
        }

        [Call]
        [Usage("Creates a global application")]
        [RequireContext(ContextType.DM | ContextType.Group)]
        private Task RegisterGlobalAsync(int maxStage, [Dense]string message,
            [CallFlag('i', "images", "Specifies a list of images to use with your application")]Uri[] images = null,
            [CallFlag('r', "relics", "Specifies how many relics you have earned")]double relics = -1,
            [CallFlag('a', "attacks", "Specifies how many attacks you aim to do per week")]int attacks = -1,
            [CallFlag('t', "taps", "Specifies how many taps you average per CQ")]int taps = -1)
            => RegisterAsync(maxStage, message, null, images, relics, attacks, taps);

        private async Task RegisterAsync(int maxStage, string message, ulong? guildId, Uri[] images, double relics, int attacks, int taps)
        {
            var current = GetRegistrations(guildId, Author.Id, !guildId.HasValue).OrderByDescending(r => r.EditTime).FirstOrDefault();
            var playerData = GetPlayers(Author.Id).FirstOrDefault();
            var isNew = current == null;

            current = current ?? new Registration
            {
                UserId = Author.Id,
                GuildId = guildId
            };
            playerData = playerData ?? new PlayerData
            {
                Id = Author.Id
            };

            current.EditTime = DateTime.Now;
            current.Message = message;
            current.Images = images;

            if (relics != -1)
                playerData.Relics = relics.Clamp(0, 2e15);
            if (attacks != -1)
                playerData.AttacksPerWeek = attacks.Clamp(0, 300);
            if (taps != -1)
                playerData.TapsPerCQ = taps.Clamp(0, 600);
            playerData.MaxStage = maxStage;

            await Database.Upsert(current);
            await Database.Upsert(playerData);

            if (isNew && guildId == null)
                await ReplyAsync("Your global application has been successful. Recruiters from any TT2 guild will be able to see your application and might choose to recruit you.", ReplyType.Success);
            else if (isNew)
                await ReplyAsync("Your application has been successful. The clan recruiter will review your application and potentially get back to you.", ReplyType.Success);
            else
                await ReplyAsync("Your global application has been successfully updated", ReplyType.Success);
        }

        [Call("View")]
        [Usage("Views your registration for this guild")]
        [RequireContext(ContextType.Guild)]
        [DefaultPermission(0)]
        Task ViewGuildRegistrationAsync([CallFlag('g', "global", "Specifies that the application is a global application.")]bool isGlobal = false)
            => ViewRegistrationAsync(Guild.Id, Author, isGlobal);

        [Call("View")]
        [Usage("Views your global registration")]
        [RequireContext(ContextType.DM | ContextType.Group)]
        Task ViewGlobalRegistrationAsync()
            => ViewRegistrationAsync(null, Author, true);

        [Call("View")]
        [Usage("View the registration for the given user")]
        [RequireContext(ContextType.Guild)]
        Task ViewGuildRegistrationAsync(IUser user,
            [CallFlag('g', "global", "Specifies that the application is a global application.")]bool isGlobal = false)
            => ViewRegistrationAsync(Guild.Id, user, isGlobal);

        async Task ViewRegistrationAsync(ulong? guildId, IUser user, bool global)
        {
            Registration current = GetRegistrations(Guild.Id, Author.Id, global).OrderByDescending(r => r.EditTime).FirstOrDefault();
            PlayerData player = GetPlayers(Author.Id).FirstOrDefault();

            if (current == null)
            {
                if (user.Id == Author.Id)
                    await ReplyAsync("You have not registered yet!", ReplyType.Error);
                else if (global)
                    await ReplyAsync("That user does not have a global registration yet!", ReplyType.Error);
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
            }.AddInlineField("Max Stage", Formatter.Beautify(player.MaxStage))
             .AddInlineField("Relics", Formatter.Beautify(player.Relics))
             .AddInlineField("CQ/Week", Formatter.Beautify(player.AttacksPerWeek))
             .AddInlineField("Taps/CQ", Formatter.Beautify(player.TapsPerCQ))
             .AddField("Images", string.Join("\n", current.Images?.Select(i => i.AbsoluteUri) ?? new string[] { "None" }));

            await ReplyAsync("", embed: builder);
        }

        [Call("Cancel")]
        [Usage("Cancels your registration for this guild")]
        [RequireContext(ContextType.Guild)]
        [DefaultPermission(0)]
        Task RemoveGuildRegistrationAsync()
            => RemoveRegistrationAsync(Guild.Id, Author);

        [Call("Cancel")]
        [Usage("Cancels your global registration")]
        [RequireContext(ContextType.DM | ContextType.Group)]
        Task RemoveGlobalRegistrationAsync()
            => RemoveRegistrationAsync(null, Author);

        [Call("Remove")]
        [Usage("Removes the registration for the given user")]
        [RequireContext(ContextType.Guild)]
        Task RemoveGuildRegistrationAsync(IUser user)
            => RemoveRegistrationAsync(Guild.Id, user);

        async Task RemoveRegistrationAsync(ulong? guildID, IUser user)
        {
            if (guildID == null && user != Author)
            {
                await ReplyAsync($"You cannot remove another users global application. Try usng `{Prefix}apply ignore <user>`.", ReplyType.Error);
                return;
            }
            await Database.Delete<Registration>(r => r.UserId == user.Id && r.GuildId == guildID);

            if (user.Id == Author.Id)
                await ReplyAsync($"You have successfully removed your {(guildID == null ? "global " : "")}application{(guildID != null ? " for this guild" : "")}", ReplyType.Success);
            else
                await ReplyAsync($"You have successfully removed the application by {user.Username} for this guild", ReplyType.Success);
        }

        [Call("Ignore")]
        [Usage("Specifies if a users global registrations should be ignored. Defaults to yes")]
        [RequireContext(ContextType.Guild)]
        async Task IgnoreUser(IUser user, bool ignore = true)
        {
            var registerSettings = SettingsManager.GetGroup<RegistrationSettings>(Guild.Id);
            if (ignore)
                registerSettings.IgnoreList.Add(user.Id);
            else
                registerSettings.IgnoreList.Remove(user.Id);

            SettingsManager.SaveGroup(Guild.Id, registerSettings);

            if (ignore)
                await ReplyAsync("That user will no longer be shown from the global listings. They will be for local ones however.", ReplyType.Success);
            else
                await ReplyAsync("That user will now be shown in global listings.", ReplyType.Success);
        }

        [Call("List")]
        [Usage("Lists all applications for this guild")]
        [RequireContext(ContextType.Guild)]
        async Task ListRegistrationsAsync(int? start = null, int? end = null,
            [CallFlag('g', "global", "Specifies that the application is a global application.")]bool isGlobal = false)
        {
            var from = start ?? 0;
            var to = end ?? from + 20;
            if (from > to)
                (from, to) = (to, from);
            to.Clamp(from, from + 30);

            var applications = GetRegistrations(Guild.Id, null, isGlobal);
            var players = GetPlayers(applications.Select(a => a.UserId).ToArray());

            var paired = applications.Join(players, a => a.UserId, p => p.Id, (a, p) => (Application: a, Player: p));

            var ignore = SettingsManager.GetGroup<RegistrationSettings>(Guild.Id).IgnoreList;
            paired = paired.Where(a => !(ignore.Contains(a.Application.UserId) && a.Application.GuildId == null))
                                       .OrderByDescending(a => a.Player.MaxStage)
                                       .ThenByDescending(a => Math.Round(Math.Sqrt(a.Player.Relics), 0))
                                       .ThenByDescending(a => a.Player.AttacksPerWeek)
                                       .ThenBy(a => a.Application.EditTime)
                                       .Skip(from);

            var table = new List<string[]> { };
            table.Add(new string[]
            {
                "#",
                "User",
                "MS",
                "Imgs",
                "Relics",
                "CQ/wk",
                "Taps/CQ",
                "Last edit",
                ""
            });
            var pos = from + 1;
            foreach (var app in paired)
            {
                var user = Client.GetUser(app.Application.UserId);
                if (user == null)
                    continue;
                table.Add(new string[]{
                    "#" + pos++.ToString(),
                    $"{user} ({user.Id})",
                    $"[{Formatter.Beautify(app.Player.MaxStage)}]",
                    (app.Application.Images?.Length ?? 0).ToString(),
                    $"#{Formatter.Beautify(app.Player.Relics)}",
                    Formatter.Beautify(app.Player.AttacksPerWeek),
                    Formatter.Beautify(app.Player.TapsPerCQ),
                    (DateTime.Now - app.Application.EditTime).Days + " day(s) ago",
                    app.Application.GuildId == null ? "-g" : ""
                });
                if (table.Count == to - from)
                    break;
            }

            var text = table.ToArray().Tableify();

            if (table.Count == 1)
                await ReplyAsync("You have no registered users!", ReplyType.Error);
            else
                await ReplyAsync($"```css\n{text}```");
        }

        [Call("Clear")]
        [Usage("Completely clears your guilds application list")]
        [RequireContext(ContextType.Guild)]
        async Task ClearRegistrationsAsync()
        {
            await Database.Delete<Registration>(r => r.GuildId == Guild.Id);
            await ReplyAsync("Your guilds registrations have been wiped", ReplyType.Success);
        }

        private IEnumerable<Registration> GetRegistrations(ulong? guildId, ulong? userId, bool includeGlobal)
        {
            if (userId == null && includeGlobal)
                return Database.Find<Registration>(r => r.GuildId == null || r.GuildId == guildId).Result;
            else if (userId == null)
                return Database.Find<Registration>(r => r.GuildId == guildId).Result;
            else if (includeGlobal)
                return Database.Find<Registration>(r => (r.GuildId == null || r.GuildId == guildId) && r.UserId == userId.Value).Result;
            else
                return Database.Find<Registration>(r => r.GuildId == guildId && r.UserId == userId.Value).Result;
        }

        private IEnumerable<PlayerData> GetPlayers(params ulong[] userids)
            => Database.FindById<PlayerData>(userids).Result;

        private void RemoveRegistration(ulong? guildid, ulong? userid)
        {
            Database.Delete<Registration>(r => r.GuildId == guildid && (!userid.HasValue || r.UserId == userid.Value)).Wait();
        }
    }
}
