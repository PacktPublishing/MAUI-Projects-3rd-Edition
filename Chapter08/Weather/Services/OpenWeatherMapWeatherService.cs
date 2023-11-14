using System;
using System.Globalization;
using Weather.Models;
using System.Text.Json;

namespace Weather.Services;

public class OpenWeatherMapWeatherService : IWeatherService
{
    public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
    {
        var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var apiKey = "{AddYourApiKeyHere}";
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

        var httpClient = new HttpClient();
        var result = await httpClient.GetStringAsync(uri);
        var data = JsonSerializer.Deserialize<WeatherData>(result);
        var forecast = new Forecast()
        {
            City = data.city.name,
            Items = data.list.Select(x => new ForecastItem()
            {
                DateTime = ToDateTime(x.dt),
                Temperature = x.main.temp,
                WindSpeed = x.wind.speed,
                Description = x.weather.First().description,
                Icon = $"http://openweathermap.org/img/w/{x.weather.First().icon}.png"
            }).ToList()
        };
        return forecast;
    }

    private DateTime ToDateTime(double unixTimeStamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}
