using System;
using System.Collections.Generic;
using Android.Locations;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Util;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;
using UK.CO.Chrisjenx.Calligraphy;

namespace AppSpeedometer
{
    [Activity(Label = "Speedometer", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, MainLauncher = true, Icon = "@drawable/speedometer")]
    public class MainActivity : Activity, ILocationListener,IDialogInterfaceOnClickListener
    {
        Location location; LocationManager lcmgr; Vibrator vibrator; Color blue = Color.ParseColor("#ffdaa520"); double Lat, Lon;
        Button BtnLocation, BtnSpeed, BtnInfoGps; TextView TxtViewMain, TxtTimer; TypeGps tGps = TypeGps.Cnull;
        int ValueAlarmSpeed = 80; bool VibSpeed = false; bool AlarmSpeed = false;

        public enum TypeGps
        {
            loaction, speed, info, Cnull
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetTheme(Android.Resource.Style.ThemeMaterialLight);
            ActionBar.SetBackgroundDrawable(new ColorDrawable(Color.OrangeRed));
            SetContentView(Resource.Layout.Main);
            //hardware
            lcmgr = GetSystemService(LocationService) as LocationManager;
            vibrator = GetSystemService(VibratorService) as Vibrator;
            //
            BtnLocation = FindViewById<Button>(Resource.Id.button1);
            BtnSpeed = FindViewById<Button>(Resource.Id.button2);
            BtnInfoGps = FindViewById<Button>(Resource.Id.button3);
            TxtViewMain = FindViewById<TextView>(Resource.Id.textView1);
            TxtTimer = FindViewById<TextView>(Resource.Id.textView2timer);

            BtnInfoGps.Background.SetColorFilter(blue, PorterDuff.Mode.Multiply);
            BtnLocation.Background.SetColorFilter(blue, PorterDuff.Mode.Multiply);
            BtnSpeed.Background.SetColorFilter(blue, PorterDuff.Mode.Multiply);

            BtnInfoGps.SetTextColor(Color.WhiteSmoke);
            BtnLocation.SetTextColor(Color.WhiteSmoke);
            BtnSpeed.SetTextColor(Color.WhiteSmoke);
            BtnInfoGps.Click += BtnInfoGps_Click; BtnLocation.Click += BtnLocation_Click; BtnSpeed.Click += BtnSpeed_Click;
            ActionBar.Title = "Speedometer";
        }

        protected override void OnStart()
        {
            base.OnStart();
            ActionBar.Title = "Speedometer";
        }
        // over ride fonts
        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        private void BtnSpeed_Click(object sender, EventArgs e)
        {
            vibrator.Vibrate(30);
            try
            {
                lcmgr.RequestLocationUpdates(LocationManager.GpsProvider, 1, 1, this);
                location = lcmgr.GetLastKnownLocation(LocationManager.GpsProvider);
                RunOnUiThread(() => OnLocationChanged(location));
                tGps = TypeGps.speed;
            }
            catch { }
        }

        private void BtnLocation_Click(object sender, EventArgs e)
        {
            vibrator.Vibrate(30);
            try
            {
                lcmgr.RequestLocationUpdates(LocationManager.NetworkProvider, 1, 1, this);
                location = lcmgr.GetLastKnownLocation(LocationManager.NetworkProvider);
                RunOnUiThread(() => OnLocationChanged(location));
                tGps = TypeGps.loaction;
            }
            catch { }
        }

        private void BtnInfoGps_Click(object sender, EventArgs e)
        {
            vibrator.Vibrate(30);
            try
            {
                lcmgr.RequestLocationUpdates(LocationManager.GpsProvider, 1, 1, this);
                location = lcmgr.GetLastKnownLocation(LocationManager.GpsProvider);
                RunOnUiThread(() => OnLocationChanged(location));
                tGps = TypeGps.info;
            }
            catch { }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add("Alarm Speed");
            menu.Add("Manager");
            menu.Add("About");
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            switch (item.TitleFormatted.ToString())
            {
                case "About":
                    AlertDialog.Builder message = new AlertDialog.Builder(this);
                    message.SetTitle("About");
                    message.SetMessage("Developer app : Mahdi khayamdar\nVisual Studio 2015 - Xamarin 4.1");
                    message.SetNeutralButton("Exit", delegate { return; });
                    message.Create(); message.Show();
                    break;
                case "Manager":
                    AlertDialog.Builder MsBox = new AlertDialog.Builder(this);
                    MsBox.SetTitle("Manager app");
                    string[] itemMs = new string[] { "Location map","Loacation access" };
                    MsBox.SetItems(itemMs, this);
                    MsBox.SetNeutralButton("Exit", delegate { return; });
                    MsBox.SetCancelable(false);
                    MsBox.Create(); MsBox.Show();
                    break;
                case "Alarm Speed":
                    MeAlarmSpeed();
                    break;
                default:
                    break;
            }
            return base.OnMenuItemSelected(featureId, item);
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            switch (which)
            {
                case 0:
                    Toast.MakeText(this, "geo:" + Lat + "," + Lon, ToastLength.Long).Show();
                    var uri = Android.Net.Uri.Parse("geo:" + Lat + "," + Lon);
                    Intent intent = new Intent(Intent.ActionView, uri);
                    //intent.SetClassName("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
                    StartActivity(intent);
                    break;
                case 1:
                    StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                    break;
                default:
                    break;
            }
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            AlertDialog.Builder message = new AlertDialog.Builder(this);
            message.SetTitle("Exit app");
            message.SetMessage("Have you exit the app ?");
            message.SetPositiveButton("Ok", delegate { Finish(); });
            message.SetNegativeButton("No", delegate { return; });
            message.SetCancelable(false);
            message.Create(); message.Show();
            return base.OnKeyDown(keyCode, e);
        }

        public void MeAlarmSpeed()
        {
            AlertDialog.Builder message = new AlertDialog.Builder(this);
            message.SetTitle("هشدار سرعت");
            View viewAlert = View.Inflate(this, Resource.Layout.UiSpeedCheck, null);

            CheckBox CheckBoxVib = (CheckBox)viewAlert.FindViewById(Resource.Id.CheckVib);
            CheckBoxVib.CheckedChange += delegate { VibSpeed = CheckBoxVib.Checked; };
            RadioGroup radiogroup = (RadioGroup)viewAlert.FindViewById(Resource.Id.radioGroupSpeed);
            radiogroup.CheckedChange += Radiogroup_CheckedChange;

            message.SetPositiveButton("اجرا", delegate { AlarmSpeed = true;
                try {
                    lcmgr.RequestLocationUpdates(LocationManager.GpsProvider, 1, 1, this);
                    location = lcmgr.GetLastKnownLocation(LocationManager.GpsProvider);
                    RunOnUiThread(() => OnLocationChanged(location));
                    tGps = TypeGps.speed;
                }
                catch { Log.Error("Tag Alarm speed", "Run gps ...");  }

            });
            message.SetView(viewAlert);
            message.SetNegativeButton("لغو", delegate { });
            message.SetNeutralButton("لغو هشدار سرعت", delegate { AlarmSpeed = false; });
            message.SetCancelable(false);
            message.Create();message.Show();
        }

        private void Radiogroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            switch (e.CheckedId)
            {
                case Resource.Id.radioButtonS80:
                    ValueAlarmSpeed = 80;
                    break;
                case Resource.Id.radioButtonS100:
                    ValueAlarmSpeed = 100;
                    break;
                case Resource.Id.radioButtonS120:
                    ValueAlarmSpeed = 120;
                    break;
                case Resource.Id.radioButtonS160:
                    ValueAlarmSpeed = 160;
                    break;
                case Resource.Id.radioButtonS200:
                    ValueAlarmSpeed = 200;
                    break;
                default:
                    break;
            }
        }

        public void OnLocationChanged(Location location)
        {
            TxtViewMain.SetTextColor(Color.Black);
            switch (tGps)
            {
                case TypeGps.loaction:
                    TxtViewMain.Text = "Latitude : " + location.Latitude + "\nLongitude : " + location.Longitude;
                    Lat = location.Latitude;
                    Lon = location.Longitude;
                    break;
                case TypeGps.speed:

                    TxtViewMain.Text = "Speed - m/s : " + Convert.ToSingle(location.Speed) + "\n\nSpeed - km/h : " + Convert.ToSingle(location.Speed * 3.6);

                    if (AlarmSpeed == true)
                    {
                        int Speed = Convert.ToInt16(location.Speed * 3.6);
                        if (Speed > ValueAlarmSpeed)
                        {
                            TxtViewMain.SetTextColor(Color.LimeGreen);
                        }
                        else if (Speed < ValueAlarmSpeed)
                        {
                            TxtViewMain.SetTextColor(Color.Red);
                            if (VibSpeed)
                                vibrator.Vibrate(200);
                        }
                        else if (Speed == ValueAlarmSpeed)
                        {
                            TxtViewMain.SetTextColor(Color.Rgb(19, 190, 255));
                            if (VibSpeed)
                                vibrator.Vibrate(50);
                        }
                    }

                    break;
                case TypeGps.info:
                    TxtViewMain.Text = "Altitude - m : " + location.Altitude + "\nBearing (in degrees) : " + location.Bearing + "\nTime : " + location.Time;
                    break;
                case TypeGps.Cnull:
                    break;
                default:
                    break;
            }
        }

        public void OnProviderDisabled(string provider)
        {
            Toast.MakeText(this, "Gps disabled", ToastLength.Short).Show();
        }

        public void OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "Gps Enabled", ToastLength.Short).Show();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Toast.MakeText(this, "GPS", ToastLength.Short).Show();
        }

    }
}