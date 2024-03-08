using Microsoft.Extensions.Logging;
using NetMongoMigrator.Core;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace NetMongoMigrator.Console.Down
{
    internal class DownCommand : Command<DownSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] DownSettings settings)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole());

            var logger = loggerFactory.CreateLogger<Migrator>();

            var configuration = new MigratorConfiguration
            {
                ConnectionString = settings.DatabaseConnectionString,
                DatabaseName = settings.DatabaseName
            };

            var migrator = new MigratorBuilder()
                .SetConfiguration(configuration)
                .AddMigrationsFromAssembly(ConsoleMigratorSettings.AssemblyToScan!)
                .SetLogger(logger)
                .Build();

            var version = GetVersion(settings);

            var migrateTask = migrator.ExecuteMigrationsDown(version);
            migrateTask.Wait();

            return 0;
        }

        private static int? GetVersion(DownSettings settings)
        {
            if (settings.Version is null)
                return null;

            if (int.TryParse(settings.Version, out var v))
                return v;

            throw new ArgumentException("Version is not a number.");
        }
    }
}
