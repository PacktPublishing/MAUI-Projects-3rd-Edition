[assembly: Android.App.UsesPermission(Android.Manifest.Permission.ReadMediaImages)]
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.ReadExternalStorage, MaxSdkVersion = 32)]

namespace GalleryApp;

using Android.OS;

internal partial class AppPermissions
{
    internal partial class AppPermission : Permissions.Photos
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                List<(string androidPermission, bool isRuntime)> perms = new();

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                    perms.Add((global::Android.Manifest.Permission.ReadMediaImages, true));
                else
                    perms.Add((global::Android.Manifest.Permission.ReadExternalStorage, true));

                return perms.ToArray();
            }
        }
    }
}
