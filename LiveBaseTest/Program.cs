using System;
using System.Threading;
using TitanBot;
using TitanBot.Formatting;
using TitanBot.Logging;

namespace LiveBaseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(100, 100);
            var bot = new BotClient(m =>
            {
                m.Map<ILogger, ConsoleLogger>();
                m.Map<ValueFormatter, Formatter>();
            });
            bot.CommandService.Install(bot.DefaultCommands);
            bot.StartAsync(c =>
            {
                if (string.IsNullOrWhiteSpace(c))
                {
                    Console.WriteLine("Please enter a bot token:");
                    return Console.ReadLine();
                }
                return c;
            }).Wait();
            bot.UntilOffline.Wait();
        }

        class ConsoleLogger : Logger
        {
            protected override LogSeverity LogLevel => LogSeverity.Critical | LogSeverity.Debug | LogSeverity.Error | LogSeverity.Info | LogSeverity.Verbose;

            protected override void WriteLog(ILoggable entry)
            {
                Console.WriteLine(entry);
                base.WriteLog(entry);
            }
        }
    }
}
