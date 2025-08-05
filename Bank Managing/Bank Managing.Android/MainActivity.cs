using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Essentials;
using Android.Widget;
using Bank_Managing.Services;
using Android.Views;

[assembly: Xamarin.Forms.Dependency(typeof(Bank_Managing.Droid.MainActivity.Toaster))]

namespace Bank_Managing.Droid
{
    [Activity(Label = "Bank_Managing", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int RequestSmsPermissionId = 1001;

        string[] permissions = {
            Android.Manifest.Permission.ReadSms,
            Android.Manifest.Permission.ReceiveSms
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Android.Manifest.Permission.ReadSms) != Permission.Granted)
                {
                    RequestPermissions(permissions, RequestSmsPermissionId);
                }
                //Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
            }
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#2E74FF"));
            Window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#BBBBBB"));
            //Window.StatusBarColor = Color.Beige


            LoadApplication(new App());
        }

        void RequestSmsPermission()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Android.Manifest.Permission.ReadSms) != Permission.Granted)
                {
                    RequestPermissions(new string[]
                    {
                        Android.Manifest.Permission.ReadSms,
                        Android.Manifest.Permission.ReceiveSms
                    }, RequestSmsPermissionId);
                }
            }
        }

        public class Toaster : IToast
        {
            public void MakeToast(string message)
            {
                Toast.MakeText(Platform.AppContext, message, ToastLength.Long).Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}