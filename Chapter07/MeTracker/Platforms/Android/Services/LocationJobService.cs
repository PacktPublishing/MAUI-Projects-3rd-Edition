using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using MeTracker.Repositories;

namespace MeTracker.Platforms.Android.Services;

[Service(Name = "MeTracker.PLatforms.Android.Services.LocationJobService", Permission = "android.permission.BIND_JOB_SERVICE")]
internal class LocationJobService : JobService, ILocationListener
{
    private ILocationRepository locationRepository;
    private static LocationManager locationManager;

    public LocationJobService()
    {
        locationRepository = MauiApplication.Current.Services.GetService<ILocationRepository>();
    }

    public override bool OnStartJob(JobParameters @params)
    {
        PermissionStatus status = PermissionStatus.Unknown;
        Task.Run(async () => status = await AppPermissions.CheckRequiredPermissionAsync()).Wait();
        if (status == PermissionStatus.Granted)
        {
            locationManager = (LocationManager)ApplicationContext.GetSystemService(Context.LocationService);
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 1000L, 0.1f, this);

            return true;
        }
        return false;
    }

    public override bool OnStopJob(JobParameters @params)
    {
        return true;
    }

    public void OnLocationChanged(global::Android.Locations.Location location)
    {
        var newLocation = new Models.Location(location.Latitude, location.Longitude);
        locationRepository.SaveAsync(newLocation);
    }

    public void OnProviderDisabled(string provider)
    {
    }

    public void OnProviderEnabled(string provider)
    {
    }

    public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
    {
    }
}

