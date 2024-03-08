# NetMongoMigrator

MongoDB migration library for .NET


## Getting Started

Migrator executes script-based migrations defined by classes implementing interface `IMigration`:


    using MongoDB.Driver;
    using NetMongoMigrator.Core;

    namespace NetMongoMigrator.Console.Example.Migrations
    {
        internal class Migration_0001_Example : IMigration
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

Migration definition:
* `Id` - have to be unique. Migrator executes migrations sorting them ascending by id.
* `Up(IMongoDatabase mongoDatabase, CancellationToken cancellationToken)` - method called when migration is executed up.
* `Down(IMongoDatabase mongoDatabase, CancellationToken cancellationToken)` - method called when migration is executed down.

Currently library allows you to launch migrations in two ways.
* Using core component directly
* Using console implementation


### I. Using core component directly

Install nuget package `NetMongoMigrator.Core`.

Prepare migrator configuration class `MigratorConfiguration`:

    var configuration = new MigratorConfiguration
    {
        MigrationsTableName = "Example Migrations Table Name",
        ConnectionString = "Example Connection String",
        DatabaseName = "Example Database Name"
    };

Build migrator:

    var migrator = new MigratorBuilder()
        .SetConfiguration(configuration)
        .SetMigrations(new [] { new Migration() })
        .SetLogger(logger)
        .Build();

You can also scan assembly for migrations:

    var migrator = new MigratorBuilder()
        .SetConfiguration(configuration)
        .AddMigrationsFromAssembly(typeof(Program).Assembly)
        .SetLogger(logger)
        .Build();

⚠️ If you not specify configuration, migrations or logger code will throw `MigratorNotConfiguredCorrectlyException`.

Then run migrator up to latest version:

    await migrator.ExecuteMigrationsUp();

Or up to specified migration id:

    await migrator.ExecuteMigrationsUp(5);

You can also run migrator down to latest version:

    await migrator.ExecuteMigrationsDown();

Or down to specified migration id:

    await migrator.ExecuteMigrationsDown(5);

### II. Using console implementation

Create new console project and install nuget package `NetMongoMigrator.Console`.

Inside `Program.cs` launch migrator:

    using NetMongoMigrator.Console;

    var consoleMigrator = new ConsoleMigrator();
    return consoleMigrator.Run(args, typeof(Program).Assembly);

Migrator will automaticly scan for migrations inside specified assembly.

You can find example in `Examples/NetMongoMigrator.Console.Example`.

To launch migrator compile your console app and execute it:

    migratorApp.exe up [connection string] [database name]

You can also migrate to specified migration id:

    migratorApp.exe up [connection string] [database name] -v <migration id>

And migrate down:

    migratorApp.exe down [connection string] [database name] -v <migration id>


## Plans for future:

1. Handling migrations exceptions
1. NetMongoMigrator.MongoDbDriverExtensions - extensions for `MongoDb.Driver` making migrations easier to implement
