using System;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MeTracker.Controls;

public class CustomMap : Microsoft.Maui.Controls.Maps.Map
{
    public readonly static BindableProperty PointsProperty = BindableProperty.Create(nameof(Points), typeof(List<Models.Point>), typeof(CustomMap), new List<Models.Point>(), propertyChanged: OnPointsChanged);

    private static void OnPointsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var map = bindable as Map;
        if (newValue == null) return;
        if (map is null) return;
        foreach (var point in newValue as List<Models.Point>)
        {
            // Instantiate a Circle
            Circle circle = new()
            {
                Center = new Location(point.Location.Latitude, point.Location.Longitude),
                Radius = new Distance(200),
                StrokeColor = Color.FromArgb("#88FF0000"),
                StrokeWidth = 0,
                FillColor = point.Heat
            };

            // Add the Circle to the map's MapElements collection
            map.MapElements.Add(circle);
        }
    }

    public List<Models.Point> Points
    {
        get => GetValue(PointsProperty) as List<Models.Point>;
        set => SetValue(PointsProperty, value);
    }

    public CustomMap()
    {
        IsScrollEnabled = true;
        IsShowingUser = true;
    }
}
