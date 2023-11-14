using SQLite;

namespace MeTracker.Repositories;

public class LocationRepository : ILocationRepository
{
    public async Task<List<Location>> GetAllAsync()
    {
        await CreateConnectionAsync();
        var locations = await connection.Table<Location>().ToListAsync();

        return locations;
    }

    public async Task SaveAsync(Models.Location location)
    {
        await CreateConnectionAsync();
        await connection.InsertAsync(location);
    }

    private SQLiteAsyncConnection connection;
    private async Task CreateConnectionAsync()
    {
        if (connection != null)
        {
            return;
        }

        var databasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Locations.db");

        connection = new SQLiteAsyncConnection(databasePath);
        await connection.CreateTableAsync<Location>();
    }
}
