﻿using MongoDB.Driver;

namespace MongoMigrator.Core.Extensions
{
    internal static class MongoDatabaseExtensions
    {
        public static bool CollectionExists(this IMongoDatabase mongoDatabase, string collectionName)
        {
            return mongoDatabase.ListCollectionNames().ToList().Contains(collectionName);
        }
    }
}
