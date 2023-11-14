using CommunityToolkit.Mvvm.ComponentModel;
using MeTracker.Repositories;
using MeTracker.Services;

namespace MeTracker.ViewModels;

public partial class MainViewModel : ViewModel
{
    private readonly ILocationRepository locationRepository; 
    private readonly ILocationTrackingService locationTrackingService;

    [ObservableProperty]
    private List<Models.Point> points;

    public MainViewModel(ILocationTrackingService locationTrackingService, ILocationRepository locationRepository)
    {
        this.locationTrackingService = locationTrackingService; 
        this.locationRepository = locationRepository;
        MainThread.BeginInvokeOnMainThread(async() =>
        {
            locationTrackingService.StartTracking();
            await LoadDataAsync();
        });
    }

    private async Task LoadDataAsync()
    {
        var locations = await locationRepository.GetAllAsync();
        var pointList = new List<Models.Point>();

        foreach (var location in locations)
        {
            //If no points exist, create a new one and continue to the next location in the list
            if (!pointList.Any())
            {
                pointList.Add(new Models.Point() { Location = location });
                continue;
            }

            var pointFound = false;

            //try to find a point for the current location
            foreach (var point in pointList)
            {
                var distance = Location.CalculateDistance(
                        new Location(point.Location.Latitude, point.Location.Longitude), 
                        new Location(location.Latitude, location.Longitude), 
                        DistanceUnits.Kilometers);

                if (distance < 0.2)
                {
                    pointFound = true; 
                    point.Count++;
                    break;
                }
            }

            //if no point is found, add a new Point to the list of points
            if (!pointFound)
            {
                pointList.Add(new Models.Point() { Location = location });
            }

            // Next section of code goes here
            if (pointList == null || !pointList.Any())
            {
                return;
            }

            var pointMax = pointList.Select(x => x.Count).Max();
            var pointMin = pointList.Select(x => x.Count).Min();
            var diff = (float)(pointMax - pointMin);

            // Last section of code goes here
            foreach (var point in pointList)
            {
                var heat = (2f / 3f) - ((float)point.Count / diff); 
                point.Heat = Color.FromHsla(heat, 1, 0.5);
            }

        }
        Points = pointList;
    }

}