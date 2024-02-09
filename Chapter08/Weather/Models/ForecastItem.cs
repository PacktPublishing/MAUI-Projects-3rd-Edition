using System;
using System.Collections.Generic;

namespace Weather.Models;

public class ForecastItem
{
    public DateTime DateTime { get; set; }
    public string TimeAsString => DateTime.ToShortTimeString();
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}