using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace WeatherApplication
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

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
        }



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            InitObjects();
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
            DisplayData();
        }



        private async void DisplayData()
        {
            var info = await ApiHelper.GetCurrentWeatherData();
            if (info == null) return;
            // Toast.MakeText(this, $"{info.CityName} Coords:Lat{info.Lat} Long:{info.Long}", ToastLength.Long).Show();


            lblCurrentTemp.Text = info.CurrentTemp.ToString() + info.TempUnit;

            lblMinTemp.Text = GetString(Resource.String.min_temp) + info.MinTemp.ToString() + info.TempUnit;

            lblMaxTemp.Text = GetString(Resource.String.max_temp) + info.MaxTemp.ToString() + info.TempUnit;

            lblDescr.Text = GetString(Resource.String.descr)+ info.Descr + "\n"  +GetString(Resource.String.feels_like_temp)+info.FeelsLikeTemp+info.TempUnit;

            lblHumidity.Text = info.Humidity.ToString()+"%";

            lblPressure.Text = info.Pressure.ToString()+"hPa";

            lblWindSpeed.Text = GetString(Resource.String.wind_speed)+ info.WindSpeed.ToString() + info.SpeedUnit;

            lblWindDirection.Text = GetString(Resource.String.wind_dir)+ info.WindDeg.ToString();

            lblSunrise.Text = info.Sunrise.ToString();
            lblSunset.Text = info.Sunset.ToString();

            imgIcon.SetImageBitmap(info.Icon);

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
    }
}