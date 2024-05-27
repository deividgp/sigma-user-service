namespace Infrastructure.Persistence;

public class ApplicationDbContext
{
    private readonly IMongoDatabase _database;

    public ApplicationDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);

        IMongoCollection<User> userCollection = GetCollection<User>();

        var indexOptions = new CreateIndexOptions { Unique = true };

        List<CreateIndexModel<User>> indexModelList =
        [
            new(Builders<User>.IndexKeys.Ascending(user => user.Username), indexOptions),
            new(Builders<User>.IndexKeys.Ascending(user => user.Email), indexOptions),
        ];

        userCollection.Indexes.CreateMany(indexModelList);
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        return _database.GetCollection<T>(typeof(T).Name.ToLower() + "s");
    }
}
