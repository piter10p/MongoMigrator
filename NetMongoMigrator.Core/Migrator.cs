using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NetMongoMigrator.Core.Entities;

namespace NetMongoMigrator.Core
{
    public class Migrator
    {
        private readonly MigratorConfiguration _migratorConfiguration;
        private readonly IMigration[] _migrations;
        private readonly ILogger<Migrator> _logger;

        public Migrator(
            MigratorConfiguration migratorConfiguration,
            IMigration[] migrations,
            ILogger<Migrator> logger)
        {
            _migratorConfiguration = migratorConfiguration;
            _migrations = migrations;
            _logger = logger;
        }

        public Task ExecuteMigrationsUp(int? migrationId = null, CancellationToken cancellationToken = default)
        {
            return ExecuteMigrations(MigrationsExecutionDirection.Up, migrationId, cancellationToken);
        }

        public Task ExecuteMigrationsDown(int? migrationId = null, CancellationToken cancellationToken = default)
        {
            return ExecuteMigrations(MigrationsExecutionDirection.Down, migrationId, cancellationToken);
        }

        private async Task ExecuteMigrations(MigrationsExecutionDirection direction, int? migrationId, CancellationToken cancellationToken)
        {
            if (direction != MigrationsExecutionDirection.Up && direction != MigrationsExecutionDirection.Down)
                throw new ArgumentException($"Argument {direction} value is invalid: {direction}", nameof(direction));

            _logger.LogInformation("Connectiong to database.");
            var client = new MongoClient(_migratorConfiguration.ConnectionString);

            _logger.LogInformation($"Getting or creating '{_migratorConfiguration.DatabaseName}' database.");
            var database = client.GetDatabase(_migratorConfiguration.DatabaseName);

            var migrationsCollection = MigrationsGetter.GetOrCreateMigrationsCollection(database, _migratorConfiguration);
            var executedMigrationsIds = ExecutedMigrationsGetter.GetExecutedMigrationsIds(migrationsCollection);

            var migrationsToExecute = Array.Empty<IMigration>();

            if (direction == MigrationsExecutionDirection.Up)
                migrationsToExecute = GetMigrationsToExecuteUp(executedMigrationsIds, migrationId);
            else
                migrationsToExecute = GetMigrationsToExecuteDown(executedMigrationsIds, migrationId);

            _logger.LogInformation($"Detected {migrationsToExecute.Length} migrations to execute.");

            foreach (var migration in migrationsToExecute)
            {
                var migrationName = migration.GetType().Name;

                if (direction == MigrationsExecutionDirection.Up)
                {
                    _logger.LogInformation($"Executing migration {migration.Id} '{migrationName}' up.");
                    await migration.Up(database, cancellationToken);
                    await migrationsCollection.InsertOneAsync(
                        new Migration(migration.Id, migrationName, DateTime.UtcNow), cancellationToken: cancellationToken);
                }
                else
                {
                    _logger.LogInformation($"Executing migration {migration.Id} '{migrationName}' down.");
                    await migration.Down(database, cancellationToken);
                    var filter = Builders<Migration>.Filter.Eq(x => x.Id, migration.Id);
                    await migrationsCollection.DeleteOneAsync(filter, cancellationToken: cancellationToken);
                }
            }

            _logger.LogInformation("Migrations executed.");
        }

        private IMigration[] GetMigrationsToExecuteUp(int[] executedMigrationsIds, int? migrationId)
        {
            var migrationsToExecute = _migrations.Where(x => !executedMigrationsIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .ToArray();

            if (migrationId is not null)
                migrationsToExecute = migrationsToExecute.Where(x => x.Id <= migrationId).ToArray();

            return migrationsToExecute;
        }

        private IMigration[] GetMigrationsToExecuteDown(int[] executedMigrationsIds, int? migrationId)
        {
            var migrationsToExecute = _migrations.Where(x => executedMigrationsIds.Contains(x.Id))
                .OrderByDescending(x => x.Id)
                .ToArray();

            if (migrationId is not null)
                migrationsToExecute = migrationsToExecute.Where(x => x.Id > migrationId).ToArray();

            return migrationsToExecute;
        }
    }
}
