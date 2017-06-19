using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBotBase;
using TitanBotBase.Dependencies;
using TT2Bot.Helpers;
using TT2Bot.Models;
using TT2Bot.TypeReaders;

namespace TT2Bot
{
    public class TT2BotClient
    {
        BotClient Client;

        public TT2BotClient(Action<IDependencyFactory> mapper)
        {
            Client = new BotClient(mapper);
            Client.Install(Assembly.GetExecutingAssembly());

            RegisterSettings();
            RegisterTypeReaders();
            PopulateMapper();
        }

        private void PopulateMapper()
        {
            Client.DependencyFactory.ConstructAndStore<TT2DataService>();
        }

        private void RegisterSettings()
        {
            Func<string, string> strLengthValidator = ((string s) => s.Length < 500 ? null : "You cannot have more than 500 characters for this setting");
            Client.SettingsManager.Register<TitanLordSettings>().WithName("TitanLord")
                                                                .WithDescription("These are the settings surrounding the `t$titanlord` command")
                                                                .WithNotes("There are several format strings you can use to have live data in your message.\n" +
                                                                           "Use `%USER%` to include the user who started the timer\n" +
                                                                           "Use `%TIME%` to include how long until the titan lord is up\n" +
                                                                           "Use `%ROUND%` for the round number\n" +
                                                                           "Use `%CQ%` for the current CQ number\n" +
                                                                           "Use `%COMPLETE%` for the time the titan lord will be up (UTC time)")
                                                                .AddSetting(s => s.TimerText, validator: strLengthValidator)
                                                                .AddSetting(s => s.InXText, validator: strLengthValidator)
                                                                .AddSetting(s => s.NowText, validator: strLengthValidator)
                                                                .AddSetting(s => s.RoundText, validator: strLengthValidator)
                                                                .AddSetting(s => s.PinTimer)
                                                                .AddSetting(s => s.RoundPings)
                                                                .AddSetting(s => s.PrePings, (TimeSpan[] t) => t.Select(v => (int)v.TotalSeconds).ToArray(), viewer: v => string.Join(", ", v.Select(t => new TimeSpan(0, 0, t))))
                                                                .AddSetting("ClanQuest", s => s.CQ, validator: v => v > 0 ? null : "Your clan quest cannot be negative")
                                                                .AddSetting("TimerChannel", s => s.Channel, (IMessageChannel c) => c?.Id, viewer: v => v == null ? null : $"<#{v}>")
                                                                .Finalise();

            Client.SettingsManager.RegisterGlobal<TT2GlobalSettings>().WithName("TT2")
                                                                      .WithDescription("These are the global settings for titanbot")
                                                                      .AddSetting(s => s.BotBugChannel, (IMessageChannel c) => c.Id, viewer: v => $"<#{v}>")
                                                                      .AddSetting(s => s.BotSuggestChannel, (IMessageChannel c) => c.Id, viewer: v => $"<#{v}>")
                                                                      .AddSetting(s => s.GHFeedbackChannel, (IMessageChannel c) => c.Id, viewer: v => $"<#{v}>")
                                                                      .AddSetting(s => s.ImageRegex)
                                                                      .AddSetting(s => s.DefaultVersion)
                                                                      .Finalise();

            Client.SettingsManager.RegisterGlobal((m, id) => m.GetCustomGlobal<TT2GlobalSettings.DataFileVersions>(),
                                                  (m, id, o) =>
                                                  {
                                                      var parent = m.GetCustomGlobal<TT2GlobalSettings>();
                                                      parent.FileVersions = o;
                                                      m.SaveCustomGlobal(parent);
                                                  }).WithName("FileVersions")
                                                    .WithDescription("These are the versions used for the data commands")
                                                    .AddSetting(s => s.Artifact)
                                                    .AddSetting(s => s.Equipment)
                                                    .AddSetting(s => s.Helper)
                                                    .AddSetting(s => s.HelperSkill)
                                                    .AddSetting(s => s.Pet)
                                                    .Finalise();
        }

        private void RegisterTypeReaders()
        {
            Client.TypeReaders.AddTypeReader<Artifact>(new ArtifactTypeReader());
            Client.TypeReaders.AddTypeReader<Pet>(new PetTypeReader());
            Client.TypeReaders.AddTypeReader<Equipment>(new EquipmentTypeReader());
            Client.TypeReaders.AddTypeReader<Helper>(new HelperTypeReader());
        }

        public async Task StartAsync(Func<string, string> getToken)
        {
            await Client.StartAsync(getToken);
        }

        public Task StopAsync()
            => Client.StopAsync();
    }
}
