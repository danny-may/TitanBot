using System;
using System.Threading.Tasks;
using TitanBotBase.Logger;
using TT2Bot;

namespace TT2BotConsole
{
    class Program
    {
        static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            var client = new TT2BotClient(f => f.Map<ILogger, Logger>());
            await client.StartAsync(current => 
            {
                if (current != null)
                    return current;
                Console.WriteLine("Please enter a bot token (leave blank to use default):");
                return Console.ReadLine();
            });

            await Task.Delay(-1);
        }
    }
}
