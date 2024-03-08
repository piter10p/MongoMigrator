using MongoDB.Driver;
using NetMongoMigrator.Core.Entities;

namespace NetMongoMigrator.Core
{
    internal static class ExecutedMigrationsGetter
    {
        public static int[] GetExecutedMigrationsIds(IMongoCollection<Migration> migrationsCollection)
        {
            var migrations = migrationsCollection.Find(Builders<Migration>.Filter.Empty).ToList();
            return migrations.Select(x => x.Id).ToArray();
        }
    }
}
