using MongoDB.Driver;
using NetMongoMigrator.Core.Entities;
using NetMongoMigrator.Core.Extensions;

namespace NetMongoMigrator.Core
{
    internal static class MigrationsGetter
    {
        public static IMongoCollection<Migration> GetOrCreateMigrationsCollection(
            IMongoDatabase mongoDatabase, MigratorConfiguration configuration)
        {
            if (!mongoDatabase.CollectionExists(configuration.MigrationsTableName))
            {
                Console.WriteLine("No migrations table. Creating.");
                mongoDatabase.CreateCollection(configuration.MigrationsTableName);
            }

            return mongoDatabase.GetCollection<Migration>(configuration.MigrationsTableName);
        }
    }
}
