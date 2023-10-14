﻿using MongoMigrator.Console.Up;
using Spectre.Console.Cli;
using System.Reflection;

namespace MongoMigrator.Console
{
    public class ConsoleMigrator
    {
        public int Run(IEnumerable<string> args, Assembly assemblyToScan)
        {
            ConsoleMigratorSettings.AssemblyToScan = assemblyToScan;

            var app = new CommandApp();
            app.Configure(c => c
                .AddCommand<UpCommand>("up"));
            return app.Run(args);
        }
    }
}
