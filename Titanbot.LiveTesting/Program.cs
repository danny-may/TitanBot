using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using Titanbot.Core;
using Titanbot.Core.Database;
using Titanbot.Core.Database.Interfaces;
using Titanbot.Core.Logging;
using Titanbot.Core.Logging.Interfaces;

namespace Titanbot.LiveTesting
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Temproary fix for a bug in .netcore 2.0 https://github.com/dotnet/project-system/issues/2239
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            var services = new ServiceCollection();

            services.AddSingleton(Configuration.Load())
                    .AddSingleton<ILogger, Logger>()
                    .AddSingleton<IDatabase, Database>()
                    .AddSingleton<Random>();

            var provider = services.BuildServiceProvider();
        }
    }
}