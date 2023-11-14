using System.Threading.Tasks;
using Weather.Models;

namespace Weather.Services;

public interface IWeatherService
{
    Task<Forecast> GetForecastAsync(double latitude, double longitude);
}
