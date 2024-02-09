namespace MeTracker.Repositories;

public interface ILocationRepository
{
    Task<List<Location>> GetAllAsync();

    Task SaveAsync(Models.Location location);
}
