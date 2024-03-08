using MongoDB.Driver;
using NetMongoMigrator.Core;

namespace NetMongoMigrator.Console.Example.Migrations
{
    internal class Migration_0001_Initial : IMigration
    {
        public int Id => 1;

        public async Task Up(IMongoDatabase mongoDatabase, CancellationToken cancellationToken)
        {
            await mongoDatabase.CreateCollectionAsync("TestCollection1", cancellationToken: cancellationToken);
            await mongoDatabase.CreateCollectionAsync("TestCollection2", cancellationToken: cancellationToken);
            await mongoDatabase.CreateCollectionAsync("TestCollection3", cancellationToken: cancellationToken);
        }

        public async Task Down(IMongoDatabase mongoDatabase, CancellationToken cancellationToken)
        {
            await mongoDatabase.DropCollectionAsync("TestCollection1", cancellationToken: cancellationToken);
            await mongoDatabase.DropCollectionAsync("TestCollection2", cancellationToken: cancellationToken);
            await mongoDatabase.DropCollectionAsync("TestCollection3", cancellationToken: cancellationToken);
        }
    }
}
