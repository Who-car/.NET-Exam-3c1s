using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Backend.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ConnectionString");
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(configuration.GetSection("DatabaseName").Value);
    }
    
    public IMongoCollection<T> GetCollection<T>(string name) =>
        _database.GetCollection<T>(name);
}