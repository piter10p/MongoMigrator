using NetMongoMigrator.Console.Down;
using NetMongoMigrator.Console.Up;
using Spectre.Console.Cli;
using System.Reflection;

namespace NetMongoMigrator.Console
{
    public class ConsoleMigrator
    {
        public int Run(IEnumerable<string> args, Assembly assemblyToScan)
        {
            ConsoleMigratorSettings.AssemblyToScan = assemblyToScan;

            var app = new CommandApp();
            app.Configure(c =>
            {
                c.AddCommand<UpCommand>("up");
                c.AddCommand<DownCommand>("down");
            });

            return app.Run(args);
        }
    }
}
