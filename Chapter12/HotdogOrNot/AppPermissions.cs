
namespace HotdogOrNot;

internal partial class AppPermissions
{
    public static async Task<PermissionStatus> CheckRequiredPermissionAsync<TPermission>() where TPermission : Permissions.BasePermission, new() => await Permissions.CheckStatusAsync<TPermission>();

    public static async Task<PermissionStatus> CheckAndRequestRequiredPermissionAsync<TPermission>() where TPermission : Permissions.BasePermission, new()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<TPermission>();

        if (status == PermissionStatus.Granted)
            return status;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            // Prompt the user to turn on in settings
            // On iOS once a permission has been denied it may not be requested again from the application
            await App.Current.MainPage.DisplayAlert("Required App Permissions", "Please enable all permissions in Settings for this App, it is useless without them.", "Ok");
        }

        if (Permissions.ShouldShowRationale<TPermission>())
        {
            // Prompt the user with additional information as to why the permission is needed
            await App.Current.MainPage.DisplayAlert("Required App Permissions", "This is a photo based app, without these permissions it is useless.", "Ok");
        }

        status = await MainThread.InvokeOnMainThreadAsync(Permissions.RequestAsync<TPermission>);
        return status;
    }

}


