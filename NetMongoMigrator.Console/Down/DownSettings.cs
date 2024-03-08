using Spectre.Console.Cli;
using System.ComponentModel;

namespace NetMongoMigrator.Console.Down
{
    internal class DownSettings : CommandSettings
    {
        [CommandArgument(0, "[Database Connection String]")]
        public string DatabaseConnectionString { get; set; } = string.Empty;

        [CommandArgument(1, "[Database Name]")]
        public string DatabaseName { get; set; } = string.Empty;

        [CommandOption("-v|--version <migrationId>")]
        [Description("Id of migration to which to migrate")]
        [DefaultValue(null)]
        public string? Version { get; init; }
    }
}
