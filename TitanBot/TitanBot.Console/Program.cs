using Discord;
using System.Threading.Tasks;
using TitanBot.Core.Services.Setting;

namespace TitanBot.Console
{
    internal class Program
    {
        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            //var client = new DefaultTitanBotClient(LogSeverity.Debug);
            //await client.StartAsync();
            //await client.WhileRunning;

            var factory = DefaultTitanBotClient.GetDefaultInjector(LogSeverity.Debug);
            var settings = factory.GetRequiredInstance<ISettingService>();

            var context = settings.GetGlobalContext();

            var boundModel = context.GetModel<BoundModel>();

            boundModel.Name = "test";
            boundModel.Child = new ChildObj();
            boundModel.Child.Server = 100;

            var boundValue = context.Get<ChildObj>("Child");

            boundValue.Server = 1100;

            var looseValue = context.Get<string>("Name");

            var looseModel = context.GetModel<BoundModel>();
            looseModel.Name = "failure";

            await Task.Delay(0);
        }
    }
}