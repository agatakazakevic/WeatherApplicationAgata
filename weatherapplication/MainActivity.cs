using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Android.Views;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using System;
using weatherapplication.Fragments;
using Android.Content;
using Android.Views.InputMethods;
using CoordinateSharp;

namespace weatherapplication
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        TextView lblcurrenttemp;
        fragmentone fragment;
        TextView lblmaxtemp;
        TextView lblmintemp;
        TextView lbldescription;
        TextView lblhumidity;
        TextView lblpressure;
        TextView lblwindspeed;
        TextView lblwinddirection;
        TextView lblsunrise;
        TextView lblsunset;
        ImageView imgicon;
        ImageView photo;
        float tempreture;
        string data;
        ListView lstforecast;
        Android.Support.V7.Widget.SearchView searchView;
        SwipeRefreshLayout swipelayout;
        InputMethodManager imm;
        private void initobjects()
        {

            lblcurrenttemp = FindViewById<TextView>(Resource.Id.temp);
            lblmaxtemp = FindViewById<TextView>(Resource.Id.tempMax);
            lblmintemp = FindViewById<TextView>(Resource.Id.tempMin);
            lbldescription = FindViewById<TextView>(Resource.Id.descrWeather);
            lblhumidity = FindViewById<TextView>(Resource.Id.humidity);
            lblpressure = FindViewById<TextView>(Resource.Id.pressure);
            lblwindspeed = FindViewById<TextView>(Resource.Id.windSpeed);
            lblwinddirection = FindViewById<TextView>(Resource.Id.windDeg);
            lblsunrise = FindViewById<TextView>(Resource.Id.sunrise);
            lblsunset = FindViewById<TextView>(Resource.Id.sunset);
            imgicon = FindViewById<ImageView>(Resource.Id.imgWeather);
           
            swipelayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipelayout);
            swipelayout.Refresh += delegate
            {
                DisplayData(string.Empty);
            };
            lstforecast = FindViewById<ListView>(Resource.Id.lstforecast);

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            apphelper.firstrun = true;
             imm = GetSystemService(InputMethodService) as InputMethodManager;
           // apphelper.lastlanguage=apphelper.sharedpref.GetString()
        }

        protected override void OnRestart()
        {
            if (apphelper.languagechanged && apphelper.changelanguage(this))
            {
                this.Recreate();
            }
            base.OnRestart();
            
        }
        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var profiles = e.ConnectionProfiles;
            bool profileenabled = false;
            foreach (var item in profiles)
            {
                if (item==ConnectionProfile.Cellular || item ==ConnectionProfile.WiFi)
                {
                    profileenabled = true;
                    break;
                }

            }
            if (profileenabled)
            {
                if (e.NetworkAccess==NetworkAccess.Internet)
                {
                    DisplayData(string.Empty);
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            
            SetContentView(Resource.Layout.Main);
            initobjects();
        }
        protected override async void OnStart()
        {
            base.OnStart();
            var rez = await CheckRequiredPermissions();
            if (!rez)
            {
                this.Finish();
                return;
            }
            if (!checkconnected()) return;
            apphelper.changelanguage(this);
           //this.Recreate();
          //  if (apphelper.prefchanged || apphelper.firstrun)
            //{
                DisplayData(string.Empty);
          //  }
            if (apphelper.firstrun)
            {
                apphelper.firstrun = false;
            }
            
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.MainMenu, menu);

            var searchItem = menu.FindItem(Resource.Id.action_search);
            
            var test = MenuItemCompat.GetActionView(searchItem);
            searchView = test.JavaCast<Android.Support.V7.Widget.SearchView>();
            searchView.QueryTextSubmit += (sender, args) =>
             {
                 DisplayData(args.NewText);
                 //hide keyboard
                
                 imm.HideSoftInputFromWindow(searchView.WindowToken, HideSoftInputFlags.None);
                 //close searchview
                 searchView.SetIconifiedByDefault(true);
                 searchView.OnActionViewCollapsed();
             };


            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            if (item.ItemId==Resource.Id.actionrefresh)
           {
                swipelayout.Refreshing = true;
                DisplayData(string.Empty);
                Toast.MakeText(this, GetString(Resource.String.action_refresh), ToastLength.Short).Show();
            }
            else if (item.ItemId==Resource.Id.action_settings)
            {
                StartActivity(new Intent(this, typeof(SettingActivity)));
            }
            return base.OnOptionsItemSelected(item);
        }
        private bool checkconnected()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {

                return true;
            }
           
                swipelayout.Refreshing = false;
                return false;
            
        }

        private async void DisplayData(string searchcity)
        {
            var info = await APIhelper.GetCurrentWeatherData(searchcity);
            if (info==null)
            {
                swipelayout.Refreshing = false;
                Console.WriteLine("=====================");
                Console.WriteLine("refresh/getdata error");
                Console.WriteLine("=====================");
                return;
            }
            //Toast.MakeText(this, $"{info.CityName} coordinates {info.Lat}; {info.Long}", ToastLength.Short).Show();
            
            lblcurrenttemp.Text = info.Currenttemp.ToString();
            tempreture= float.Parse(lblcurrenttemp.Text);
            data = info.Description;
            lblmintemp.Text = GetString(Resource.String.min_temp)+ info.MinTemp.ToString()+info.TempUnit;
            lblmaxtemp.Text = GetString(Resource.String.max_temp)+info.Maxtemp.ToString()+info.TempUnit;
            lbldescription.Text = info.CityName+","+info.Country+":" + info.Description + "\n" + GetString(Resource.String.feelslike_temp) +info.FeelsLikeTemp +info.TempUnit;
            lblhumidity.Text = info.Humidity.ToString()+"%";
            lblpressure.Text = info.Pressure.ToString()+"hPa";
            lblwindspeed.Text = GetString(Resource.String.wind_speed) +info.Windspeed.ToString();
            lblwinddirection.Text = GetString(Resource.String.wind_d)+info.WindDir.ToString();
            lblsunset.Text = info.Sunset.ToString();
            lblsunrise.Text = info.Sunrise.ToString();
            imgicon.SetImageBitmap(info.Icon);
            Coordinate c = new Coordinate(info.Lat, info.Long, DateTime.Now);
            //ShowProgressDialogue("Fetching weather...");
            if (tempreture < 0 && data.Contains("snow"))
            {
                if (c.CelestialInfo.IsSunUp)
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav1);
                }

                else
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav3);
                }
            }

            if (tempreture < 0 && data.Contains("clouds"))
            {
                if (c.CelestialInfo.IsSunUp)
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav2);
                }

                else
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav4);
                   
                }
            }
            if (tempreture < 0 && data.Contains("rain"))
            {
                if (c.CelestialInfo.IsSunUp)
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav2);
                }
                else
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav4);
                }
            }
            if (tempreture < 0 && data.Contains("windy"))
            {
                if (c.CelestialInfo.IsSunUp)
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav3);
                }
                else
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav4);
                }
            }
            if (tempreture < 0 && data.Contains("sunny"))
            {
                photo = FindViewById<ImageView>(Resource.Id.photo);
                photo.SetImageResource(Resource.Drawable.pav5);
            }
            if (0 < tempreture && tempreture < 15 && data.Contains("rain"))
            {
                if (c.CelestialInfo.IsSunUp)
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav6);
                }
                else
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav8);
                }
            }
            if (0 < tempreture && tempreture < 15 && data.Contains("clouds"))
            {
                if (c.CelestialInfo.IsSunUp)
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav6);
                }
                else
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav10);
                }
            }
            if (0 < tempreture && tempreture < 15 && data.Contains("windy"))
            {
                if (c.CelestialInfo.IsSunUp)
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav7);
                }
                else
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav10);
                }
            }
            if (0 < tempreture && tempreture < 15 && data.Contains("sunny"))
            {
                photo = FindViewById<ImageView>(Resource.Id.photo);
                photo.SetImageResource(Resource.Drawable.pav9);
            }
            if (15 < tempreture && tempreture < 30 && data.Contains("rain"))
            {
                if (c.CelestialInfo.IsSunUp)
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav11);
                }
                else
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav12);
                }
            }
            if (15 < tempreture && tempreture < 30 && data.Contains("windy"))
            {
                if (c.CelestialInfo.IsSunUp)
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav14);
                }
                else
                {
                    photo = FindViewById<ImageView>(Resource.Id.photo);
                    photo.SetImageResource(Resource.Drawable.pav13);
                }
            }
            if (15 < tempreture && tempreture < 30 && data.Contains("clouds"))
            {

                photo = FindViewById<ImageView>(Resource.Id.photo);
                photo.SetImageResource(Resource.Drawable.pav14);
            }
            if (15 < tempreture && tempreture < 30 && data.Contains("sunny"))
            {
                photo = FindViewById<ImageView>(Resource.Id.photo);
                photo.SetImageResource(Resource.Drawable.pav15);
            }

            //forecast  part
            var lst = await APIhelper.GetForecastWeatherData(searchcity);
            lstforecast.Adapter = new forecastadapter(this, lst);
            swipelayout.Refreshing = false;
           // ClossProgressDialogue();

        }
        private  async Task<bool> CheckRequiredPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status==PermissionStatus.Granted)
            {
                return true;
            }
            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                Toast.MakeText(this, "permissions denied, please go to settings", ToastLength.Short).Show();
                return false;
            }
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return status == PermissionStatus.Granted ? true : false;

        }
        void ShowProgressDialogue(string status)
        {
            fragment = new fragmentone(status);
            var trans = SupportFragmentManager.BeginTransaction();
           fragment.Cancelable = false;
            fragment.Show(trans, "progress");
        }
        void ClossProgressDialogue()
        {
            if (fragment != null)
            {
                fragment.Dismiss();
                fragment = null;
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}