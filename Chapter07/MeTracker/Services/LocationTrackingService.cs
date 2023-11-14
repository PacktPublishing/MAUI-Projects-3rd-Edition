namespace MeTracker.Services;

public partial class LocationTrackingService : ILocationTrackingService
{
    public void StartTracking()
    {
        StartTrackingInternal();
    }

    partial void StartTrackingInternal();
}