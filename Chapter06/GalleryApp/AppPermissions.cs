namespace GalleryApp;

internal partial class AppPermissions
{
    internal partial class AppPermission : Permissions.Photos
    {
    }

    public static async Task<PermissionStatus> CheckRequiredPermission() => await Permissions.CheckStatusAsync<AppPermission>();

    public static async Task<PermissionStatus> CheckAndRequestRequiredPermission()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<AppPermission>();

        if (status == PermissionStatus.Granted)
            return status;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            // Prompt the user to turn on in settings
            // On iOS once a permission has been denied it may not be requested again from the application
            await App.Current.MainPage.DisplayAlert("Required App Permissions", "Please enable all permissions in Settings for this App, it is useless without them.", "Ok");
        }

        if (Permissions.ShouldShowRationale<AppPermission>())
        {
            // Prompt the user with additional information as to why the permission is needed
            await App.Current.MainPage.DisplayAlert("Required App Permissions", "This is a Photo gallery app, without these permissions it is useless.", "Ok");
        }

        status = await MainThread.InvokeOnMainThreadAsync(Permissions.RequestAsync<AppPermission>);
        return status;
    }

}
