using System;
using System.Collections.Generic;

namespace Weather.Models;

public class Forecast
{
    public string City { get; set; }
    public List<ForecastItem> Items { get; set; }
}
