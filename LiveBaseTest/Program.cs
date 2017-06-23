using System;
using TitanBot;
using TitanBot.Logger;

namespace LiveBaseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new BotClient(m => m.Map<ILogger, Logger>());
            bot.StartAsync(Console.ReadLine()).Wait();
            bot.UntilOffline.Wait();
        }

        class Logger : TitanBotLogger
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
