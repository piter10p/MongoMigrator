using MongoDB.Driver;
using NetMongoMigrator.Core;

namespace NetMongoMigrator.Console.Example.Migrations
{
    internal class Migration_0002_DeleteCollection : IMigration
    {
        public int Id => 2;

        public async Task Up(IMongoDatabase mongoDatabase, CancellationToken cancellationToken)
        {
            await mongoDatabase.DropCollectionAsync("TestCollection2", cancellationToken: cancellationToken);
        }

        public async Task Down(IMongoDatabase mongoDatabase, CancellationToken cancellationToken)
        {
            await mongoDatabase.CreateCollectionAsync("TestCollection2", cancellationToken: cancellationToken);
        }
    }
}
