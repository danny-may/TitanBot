using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.GuildSpecific.Singularity
{
    public class PromoteCommand : Command
    {
        private ulong[] _roleOrder = new ulong[]
        {
            307806171798962177,
            307805963295916032,
            307806447402614785,
            307806702151925760,
            307807022903197698,
            312177555379585024,
        };

        public PromoteCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            GuildRestrictions = new ulong[] { 307803032534646785 };

            Calls.AddNew(a => Promote((SocketGuildUser)a[0]))
                 .WithArgTypes(typeof(SocketGuildUser))
                 .WithItemAsParams(0);

            Description = "Promotes a user up the ranks";
        }

        private async Task Promote(SocketGuildUser user)
        {
            var callingUser = Context.User as SocketGuildUser;
            if (callingUser == null)
            {
                await ReplyAsync("An error occured, please try again later.", ReplyType.Error);
                return;
            }

            var userTopRole = callingUser.Roles.OrderBy(r => r.Position).FirstOrDefault(r => _roleOrder.Contains(r.Id));
            var targetTopRole = user.Roles.OrderBy(r => r.Position).FirstOrDefault(r => _roleOrder.Contains(r.Id));

            if (userTopRole == null)
            {
                await ReplyAsync("You do not have a member rank here!", ReplyType.Error);
                return;
            }

            if (targetTopRole == null)
            {
                await ReplyAsync("They do not have a member rank here! Please give them one of the member roles to use this command on them.", ReplyType.Error);
                return;
            }

            if (userTopRole.Position <= targetTopRole.Position)
            {
                await ReplyAsync("You cannot promote someone to a rank higher than you", ReplyType.Error);
                return;
            }

            var roles = Context.Guild.Roles.Where(r => _roleOrder.Contains(r.Id)).OrderBy(r => r.Position);
            var promoRole = roles.FirstOrDefault(r => r.Position > targetTopRole.Position);

            if (promoRole == null)
            {
                await ReplyAsync("There is no higher rank for me to promote them to!", ReplyType.Error);
                return;
            }

            await user.RemoveRolesAsync(roles.Take(roles.Count() -1).Where(r => user.Roles.Contains(r)));
            await user.AddRoleAsync(promoRole);

            await MessageForPromotion(user, promoRole);
        }

        private async Task MessageForPromotion(SocketGuildUser user, SocketRole role)
        {
            string message;
            switch (role.Id)
            {
                case 307807022903197698:
                    message =$"Hey, {user.Username} welcome to Singularity! I'm going to give you some advice and tips since you're new to the clan!\n" +
                              "```diff\n" +
                              "- 1.Mute all channels except #cq-timer and #clan-announcements.\n" +
                              "- 2.Use #checklist to check in for CQ's you'll be able to join.\n" +
                              "- 3.Check out the pinned message section of each channel, some have info for the channel and this channel has info on showing and hiding certain channels, if you're confused on how to do it feel free to ask!\n" +
                              "- 4.Use !clanpw to get the clan code and password to join the clan in-game!\n" +
                              "- 5.If you get lost feel free to ask any questions in member lobby that you have, everyone is friendly here and would be happy to help. \n" +
                              "- 6.If you will be using a bunch of commands please use #bot-spam or another spam channel such as #memes, #fun-chat or #nsfw. Thanks!\n" +
                              "- 7.Remember if you change you're In-Game name, please notify us and have your Singularity server nickname match it otherwise we won't know where to put you on the damage sheet!\n" +
                              "- 8.To check the damage sheet, use !cq.It'll bring up the damage sheet link!\n" +
                              "```\n\n" +
                              "Other than that, please enjoy your stay in Singularity! Again, if you have any questions feel free to ask.We talk mostly in Discord as well!";
                    await TrySend(user.Id, message, ReplyType.Info);
                    break;
                case 307806447402614785:
                    message =$"Congratulations on being promoted **{user.Username}**! Here are the tasks you'll have as an Admin of **{Context.Guild.Name}**\n" +
                              "```diff\n" +
                              "Claiming Tasks\n" +
                              "- After the bot announces members to pick your roles you may use the commands % roleassign ss and % roleassign dmg to claim one of the tasks. (SS is short for Screenshots, DMG is short for Damage Sheet.)\n" +
                              "-If you have claimed a task make sure to do % roleassign finish when you are done.\n" +
                              "```\n" +
                              "```diff\n" +
                              "Screenshots\n" +
                              "- Take screenshot of full CQ\n" +
                              "- Stitch the screenshots together\n" +
                              "- Post stitched screenshot to #screenshots\n" +
                              "- Use !screen command. (Number of CQ required after the command)\n" +
                              "```\n" +
                              "```diff\n" +
                              "Spreadsheet\n" +
                              "- Sort members by alphabetical order by clicking on Singularity members and sorting A to Z.\n" +
                              "- Fill in damage under the according CQ, double check when finished.\n" +
                              "- Type in the boss time at the bottom of damage for the CQ.\n" +
                              "- Sort members by damage by clicking on Dmg % (Cell D) and sorting Z to A.\n" +
                              "- Copy the whole line of a member that was removed, with Ctrl +C then paste them into the removed page with Ctrl + Shift + V then fill in the blank on the primary page with ZZ.\n" +
                              "```\n" +
                              "```diff\n" +
                              "Timers\n" +
                              "- Use the channel #cq-timer to set boss timer with %tl in 00:00:00. (Hrs:Mins:Secs)\n" +
                              "- Use the channel #admin-lobby to set roleassign timer with !roleassign afterwards edit the timer with !roleassign edit 00:00:00. (Hrs:Mins:Secs) Set this timer an hour before CQ!\n" +
                              "```\n" +
                              "```diff\n" +
                              "Bored ?\n" +
                              "- Bored ? You shouldn't be, an admins job is never done.\n" +
                              "- If you see members struggling with something, help!\n" +
                              "- If you are free to interview, announce that you are in the #recruitment-lobby! Make sure you know our requirements before doing so and also be kind and patient if you can, be yourself but don't be an ass!\n" +
                              "- When the boss gets within a 1 hour gap, remind people to !check in the #checklist channel.\n" +
                              "```\n" +
                              "If you have any questions ask your fellow Admins and Grand Master Blackice.";
                    await TrySend(user.Id, message, ReplyType.Info);
                    break;
            }
            await ReplyAsync($"Successfully promoted {user.Mention} to {role.Name}", ReplyType.Success);
        }
    }
}
