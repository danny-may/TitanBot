using System.Threading.Tasks;

namespace TitanBot.Console
{
    internal class Program
    {
        private static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            var client = new DefaultTitanBotClient();
            await client.StartAsync();
            await client.WhileRunning;
        }
    }
}