using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2;
using TitanBot2.Common;

namespace TitanBot2Console
{
    class Program
    {
        private BotClient _bot;
        private string logPath = "logs/Log_" + DateTime.Now.ToString("yyyy.mm.dd_hh.mm.ss") + ".txt";
        private object _syncObj = new object();

        static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            _bot = new BotClient();

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += async (o, e) =>
            {
                await Log((e.ExceptionObject as Exception).ToString());
                if (e.IsTerminating)
                    Restart();
            };

            _bot.Logger.HandleLog += e => Log(e);
            _bot.LoggedOut += async () =>
            {
                await Log("Logged out");
                Environment.Exit(0);
            };

            if (!await _bot.StartAsync())
            {
                var config = Configuration.Instance;
                Console.WriteLine("Please enter the bot token to use:");
                config.Token = Console.ReadLine();
                config.SaveJson();
                if (!await _bot.StartAsync())
                {
                    Console.WriteLine("Startup failed. Press ENTER to exit");
                    Console.ReadLine();
                    return;
                }
            }

            await Task.Delay(-1);
        }

        public Task Log(ILoggable entry)
            => Log(entry.ToString());

        public async Task Log(string text)
        {
            await Console.Out.WriteLineAsync(text);
            lock (_syncObj)
            {
                string file = Path.Combine(AppContext.BaseDirectory, logPath);
                if (!File.Exists(file))
                {
                    string path = Path.GetDirectoryName(file);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                File.AppendAllText(file, text + Environment.NewLine);
            }
        }

        private void Restart()
        {
            if (Process.GetCurrentProcess().StartTime < DateTime.Now.AddMinutes(-2))
                Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
            else
                Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
