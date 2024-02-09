using CoreLocation;
using MeTracker.Repositories;

namespace MeTracker.Services;

public partial class LocationTrackingService : ILocationTrackingService
{
    CLLocationManager locationManager;
    ILocationRepository locationRepository;

    public LocationTrackingService(ILocationRepository locationRepository)
    {
        this.locationRepository = locationRepository;
    }

    partial void StartTrackingInternal()
    {
        locationManager = new CLLocationManager
        {
            PausesLocationUpdatesAutomatically = false,
            AllowsBackgroundLocationUpdates = true,
            DesiredAccuracy = CLLocation.AccurracyBestForNavigation,
        };

        locationManager.LocationsUpdated += async (object sender, CLLocationsUpdatedEventArgs e) =>
        {
            var lastLocation = e.Locations.Last();
            var newLocation = new Models.Location(lastLocation.Coordinate.Latitude, lastLocation.Coordinate.Longitude);

            await locationRepository.SaveAsync(newLocation);
        };

        locationManager.RequestAlwaysAuthorization();
        locationManager.StartUpdatingLocation();
    }
}
