using Android.App;
using Android.Runtime;
using Android;

[assembly: UsesPermission(Manifest.Permission.Vibrate)]
[assembly: UsesPermission("android.permission.POST_NOTIFICATIONS")]
[assembly: UsesPermission(Manifest.Permission.WakeLock)]
[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]

// Optional for exact alarms (if applicable)
[assembly: UsesPermission("android.permission.USE_EXACT_ALARM")]
[assembly: UsesPermission("android.permission.SCHEDULE_EXACT_ALARM")]

namespace C971_MobileApp
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
