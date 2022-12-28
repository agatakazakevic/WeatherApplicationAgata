using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Runtime;
using Android.Widget;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Android.Views;
using System;
using Android.Support.V4.View;
using Android.Views.InputMethods;
using Android.Content;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using Android.Preferences;
using System.Runtime.Remoting.Contexts;
using Java.Util.Prefs;
using NHibernate.Mapping;
using Java.Util;
using CoordinateSharp;

namespace WeatherApplication
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ConfigurationChanges = Android.Content.PM.ConfigChanges.Locale)]
    public class MainActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;
        TextView lblCurrentTemp;
        TextView lblMaxTemp;
        TextView lblMinTemp;
        TextView lblDescr;
        TextView lblHumidity;
        TextView lblPressure;
        TextView lblWindSpeed;
        TextView lblWindDirection;
        TextView lblSunset;
        TextView lblSunrise;
        ImageView imgIcon;
        ImageView photo;
        float tempreture;
        string data;
        ListView lstForecast;
        Button btnsavecity;
        long lat;
        long lon;
        string city;
        string intentcity;
        //SearchView searchView;
        SwipeRefreshLayout swipeLayout;
        Android.Support.V7.Widget.SearchView searchView;
        InputMethodManager imm;
        ProgressDialogueFragment progressDialogue;
        internal bool backpressed = false;
        System.Collections.ArrayList cities = new System.Collections.ArrayList();
        public static string selectedcity;
        bool savedcity = false;

        private void InitObjects()
        {
           
            lblCurrentTemp = FindViewById<TextView>(Resource.Id.temp);
            lblMaxTemp = FindViewById<TextView>(Resource.Id.tempMax);
            lblMinTemp = FindViewById<TextView>(Resource.Id.tempMin);
            lblDescr = FindViewById<TextView>(Resource.Id.descrWeather);
            lblHumidity = FindViewById<TextView>(Resource.Id.humidity);
            lblPressure = FindViewById<TextView>(Resource.Id.pressure);
            lblWindSpeed = FindViewById<TextView>(Resource.Id.windSpeed);
            lblWindDirection = FindViewById<TextView>(Resource.Id.windDeg);
            lblSunset = FindViewById<TextView>(Resource.Id.sunset);
            lblSunrise = FindViewById<TextView>(Resource.Id.sunrise);
            imgIcon = FindViewById<ImageView>(Resource.Id.imgWeather);
            lstForecast = FindViewById<ListView>(Resource.Id.lstForecast);
            btnsavecity = FindViewById < Button>(Resource.Id.btnsavecity);
            //swipeLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeLayout);
            //swipeLayout.Refresh += delegate
            // {
            //    DisplayData(string.Empty);
            // };
            btnsavecity.Click += Btnsavecity_Click;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            AppHelper.fistRun = true;
            imm = GetSystemService(InputMethodService) as InputMethodManager;
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            // Init toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();
            

        }
       
        private void Btnsavecity_Click(object sender, EventArgs e)
        {
            if (cities.Count != 0)
            {
                if (cities.Contains(city))
                {
                    return;

                }
                else
                {
                    cities.Add(city);
                }
            }
            else
            {
                cities.Add(city);
            }


        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var profiles = e.ConnectionProfiles;
            bool profileEnabled = false;
            foreach (var item in profiles)
            {
                if (item == ConnectionProfile.Cellular || item == ConnectionProfile.WiFi)
                {
                    profileEnabled = true;
                    break;
                }
            }
            if (profileEnabled)
            {
                if (e.NetworkAccess == NetworkAccess.Internet)
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
            if (backpressed==false)
            {
               
                InitObjects();
            }
            if (savedcity == true)
            {
                //intentcity = Intent.GetStringExtra("city");

                DisplayData(selectedcity);
            }





        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Console.WriteLine("===================");
            
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {

                
                intentcity = Intent.GetStringExtra("city");
                
                DisplayData(intentcity);
            }
        }
        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_pollution):
                    var intent = new Intent(this, typeof(chartactivity));
                   
                    intent.PutExtra("lat", lat);
                    intent.PutExtra("lon", lon);
                    this.StartActivity(intent);
                    backpressed = true;
                    break;
                case (Resource.Id.nav_signin):
                  //  var intent1 = new Intent(this, typeof(signinactivity));
                    //this.StartActivity(intent1);
                    break;
                case (Resource.Id.nav_cities):
                    
                    Intent intent3 = new Intent(this, typeof(savedcitiesactivity));
                    
                    intent3.PutExtra("list", JsonConvert.SerializeObject(cities));
                    this.StartActivityForResult(intent3, 0);
                    savedcity = true;
                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                    intentcity = prefs.GetString("cityname", "vilnius");
                    
                    break;
                
            }

            // Close drawer
            drawerLayout.CloseDrawers();
        }
       
        protected override async void OnStart()
        {
            base.OnStart();
            if (savedcity==true)
            {
               

                DisplayData(selectedcity);
            }
            if ( backpressed==false && savedcity==false)
            {
               
                var rez = await CheckRequiredPermissions();
                if (!rez)
                {
                    this.Finish();
                    return;
                }

                if (!CheckConnected()) return;
                AppHelper.ChangeLanguage(this);
                //this.Recreate();
                //Toast.MakeText(this, AppHelper.prefChanged.ToString(), ToastLength.Short).Show();

                // if (AppHelper.prefChanged || AppHelper.fistRun)
                DisplayData(string.Empty);
                if (AppHelper.fistRun) AppHelper.fistRun = false;
            }
           

        }

        protected override void OnRestart()
        {
            if (AppHelper.langChanged && AppHelper.ChangeLanguage(this))
                this.Recreate();
            base.OnRestart();
            
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
            if (item.ItemId == Resource.Id.action_refresh)
            {
                //swipeLayout.Refreshing = true;
                DisplayData(string.Empty);
            }
            else if (item.ItemId == Resource.Id.action_settings)
            {
                StartActivity(new Intent(this, typeof(SettingActivity)));
            }
            return base.OnOptionsItemSelected(item);
        }




        private bool CheckConnected()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                return true;
            }
          //  swipeLayout.Refreshing = false;
            return false;
        }

        private async void DisplayData(string searchCity)
        {
            ShowProgressDialogue("Fetching weather...");
            
          
                city = searchCity;
            
            var info = await ApiHelper.GetCurrentWeatherData(searchCity);
           
            if (info == null)
            {
               //swipeLayout.Refreshing = false;
                Console.WriteLine("=================");
                Console.WriteLine("Refresh/Get data Error");
                return;
            }
           
            // Toast.MakeText(this, $"{info.CityName} Coords:Lat{info.Lat} Long:{info.Long}", ToastLength.Long).Show();
            lat = (long)info.Lat;
            lon = (long)info.Long;

            lblCurrentTemp.Text = info.CurrentTemp.ToString() + info.TempUnit;
            city = info.CityName ;
            lblMinTemp.Text = GetString(Resource.String.min_temp) + info.MinTemp.ToString() + info.TempUnit;

            lblMaxTemp.Text = GetString(Resource.String.max_temp) + info.MaxTemp.ToString() + info.TempUnit;

            lblDescr.Text = info.CityName + "," + info.Country + "," + info.Descr + "\n" + "Feels like: " + info.FeelsLikeTemp + info.TempUnit;

            lblHumidity.Text = info.Humidity.ToString() + "%";

            lblPressure.Text = info.Pressure.ToString() + "hPa";

            lblWindSpeed.Text = GetString(Resource.String.wind_speed) + info.WindSpeed.ToString() + info.SpeedUnit;

            lblWindDirection.Text = GetString(Resource.String.wind_dir) + info.WindDir;

            lblSunrise.Text = info.Sunrise.ToString();
            lblSunset.Text = info.Sunset.ToString();
           
            imgIcon.SetImageBitmap(info.Icon);
            
            //forecast part

            var lst = await ApiHelper.GetForecastWeatherData(searchCity);
            lstForecast.Adapter = new ForecastAdapter(this, lst);

            //swipeLayout.Refreshing = false;
            ClossProgressDialogue();
        }

       
        private async Task<bool> CheckRequiredPermissions()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                Toast.MakeText(this, "Permission denied. Please go to setting to enable", ToastLength.Short).Show();
                return false;
            }
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            return status == PermissionStatus.Granted ? true : false;

        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        void ShowProgressDialogue(string status)
        {
            progressDialogue = new ProgressDialogueFragment(status);
            var trans = SupportFragmentManager.BeginTransaction();
            progressDialogue.Cancelable = false;
            progressDialogue.Show(trans, "progress");
        }

        void ClossProgressDialogue()
        {
            if (progressDialogue != null)
            {
                progressDialogue.Dismiss();
                progressDialogue = null;
            }
        }
    }
}
