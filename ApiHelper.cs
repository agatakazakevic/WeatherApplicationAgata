using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace WeatherApplication
{
    class ApiHelper
    {

        private static string GetTempUnitValue(UnitsOfMeasurement units)
        {
            switch (units)
            {
                case UnitsOfMeasurement.standart:
                    return "K";
                case UnitsOfMeasurement.metric:
                    return "° C";
                case UnitsOfMeasurement.imperial:
                    return "° F";
            }
            return "";
        }

        private static string GetSpeedUnitValue(UnitsOfMeasurement units)
        {
            switch (units)
            {
                case UnitsOfMeasurement.standart:
                case UnitsOfMeasurement.metric:
                    return "m/s";
                case UnitsOfMeasurement.imperial:
                    return "mi/h";
            }
            return "";
        }

        public static async Task<WeatherInfo> GetCurrentWeatherData(string searchCity)
        {
            try
            {
                string key = "566630dc8b634a403e4e0ddaf9e66564";
                string city_name = AppHelper.sharedPref.GetString(Application.Context.GetString(Resource.String.city_pref_key),
                    Application.Context.GetString(Resource.String.default_city));

                if (!string.IsNullOrEmpty(searchCity))
                {
                    city_name = searchCity;
                }

                string unit_name = AppHelper.sharedPref.GetString(Application.Context.GetString(Resource.String.unit_pref_key),
                    Application.Context.GetString(Resource.String.default_unit));

                UnitsOfMeasurement units = (UnitsOfMeasurement)Enum.Parse(typeof(UnitsOfMeasurement), unit_name);

                string lang_name = AppHelper.sharedPref.GetString(Application.Context.GetString(Resource.String.lang_pref_key),
                    Application.Context.GetString(Resource.String.default_lang));


                bool useLoc = AppHelper.sharedPref.GetBoolean(Application.Context.GetString(Resource.String.loc_pref_key), false);
                Location loc = new Location();

                string source = $"http://api.openweathermap.org/data/2.5/weather?q={city_name}&appid={key}&units={units}&lang={lang_name}";

                if (useLoc && string.IsNullOrEmpty(searchCity))
                {
                    Task.Run(async delegate
                    {
                        loc = await GetCurrentLocation();
                    }).Wait();

                    source = $"http://api.openweathermap.org/data/2.5/weather?lat={loc.Latitude}&lon={loc.Longitude}&appid={key}&units={units}&lang={lang_name}";
                }

                var data = await HttpHelper.GetDataFromAPI(source);
                if (data == null) return null;
                //try parsing data
                WeatherInfo info = new WeatherInfo();
                info.TempUnit = GetTempUnitValue(units);
                info.SpeedUnit = GetSpeedUnitValue(units);

                info.Lat = (float)data["coord"]["lat"];
                info.Long = (float)data["coord"]["lon"];
                info.CityName = data["name"].ToString();
                info.Country = data["sys"]["country"].ToString();
                info.Descr = data["weather"][0]["description"].ToString();
                info.CurrentTemp = (float)data["main"]["temp"];
                info.MaxTemp = (float)data["main"]["temp_max"];
                info.MinTemp = (float)data["main"]["temp_min"];
                info.FeelsLikeTemp = (float)data["main"]["feels_like"];
                info.Humidity = (byte)data["main"]["humidity"];
                info.Pressure = (int)data["main"]["pressure"];
                info.WindSpeed = (float)data["wind"]["speed"];
                info.WindDeg = (float)data["wind"]["deg"];
                info.WindDir = WindDegree2Direction(info.WindDeg);
                int timezone = (int)data["timezone"];
                info.Sunrise = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds((double)data["sys"]["sunrise"]).AddSeconds(timezone);
                info.Sunset = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds((double)data["sys"]["sunset"]).AddSeconds(timezone);

                Task.Run(async delegate
                {
                    //get picture
                    info.Icon = await HttpHelper.GetWeatherIcon(data["weather"][0]["icon"].ToString());
                }).Wait();

                return info;
            }
            catch (Exception ex)
            {
#if DEBUG
                Toast.MakeText(Application.Context, "Data Parsing Error:" + ex.Message, ToastLength.Long).Show();
                Console.WriteLine("=====================");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("=====================");
                return null;
#else
                Toast.MakeText(Application.Context, "Cannot parse data", ToastLength.Long).Show();
                return null;
#endif
            }
        }



        public static async Task<List<WeatherInfo>> GetForecastWeatherData(string searchCity)
        {
            try
            {
                string key = "566630dc8b634a403e4e0ddaf9e66564";
                string city_name = AppHelper.sharedPref.GetString(Application.Context.GetString(Resource.String.city_pref_key),
                    Application.Context.GetString(Resource.String.default_city));

                if (!string.IsNullOrEmpty(searchCity))
                {
                    city_name = searchCity;
                }

                string unit_name = AppHelper.sharedPref.GetString(Application.Context.GetString(Resource.String.unit_pref_key),
                    Application.Context.GetString(Resource.String.default_unit));

                UnitsOfMeasurement units = (UnitsOfMeasurement)Enum.Parse(typeof(UnitsOfMeasurement), unit_name);

                string lang_name = AppHelper.sharedPref.GetString(Application.Context.GetString(Resource.String.lang_pref_key),
                    Application.Context.GetString(Resource.String.default_lang));


                bool useLoc = AppHelper.sharedPref.GetBoolean(Application.Context.GetString(Resource.String.loc_pref_key), false);
                Location loc = new Location();

                string source = $"http://api.openweathermap.org/data/2.5/forecast?q={city_name}&appid={key}&units={units}&lang={lang_name}";

                if (useLoc && string.IsNullOrEmpty(searchCity))
                {
                    Task.Run(async delegate
                    {
                        loc = await GetCurrentLocation();
                    }).Wait();

                    source = $"http://api.openweathermap.org/data/2.5/forecast?lat={loc.Latitude}&lon={loc.Longitude}&appid={key}&units={units}&lang={lang_name}";
                }

                var data = await HttpHelper.GetDataFromAPI(source);
                if (data == null) return null;
                //try parsing data
                List<WeatherInfo> infoLst = new List<WeatherInfo>();
                var cnt = (int)data["cnt"];
                for (int i = 0; i < cnt; i++)
                {
                    WeatherInfo info = new WeatherInfo();
                    info.TempUnit = GetTempUnitValue(units);
                    info.Descr =data["list"][i]["dt_txt"] +"\n"+ data["list"][i]["weather"][0]["description"].ToString();
                    info.CurrentTemp = (float)data["list"][i]["main"]["temp"];
                    Task.Run(async delegate
                    {
                    //get picture
                    info.Icon = await HttpHelper.GetWeatherIcon(data["list"][i]["weather"][0]["icon"].ToString());
                    }).Wait();
                    infoLst.Add(info);
                }
                return infoLst;
            }
            catch (Exception ex)
            {
#if DEBUG
                Toast.MakeText(Application.Context, "Data Parsing Error:" + ex.Message, ToastLength.Long).Show();
                Console.WriteLine("=====================");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("=====================");
                return null;
#else
                Toast.MakeText(Application.Context, "Cannot parse data", ToastLength.Long).Show();
                return null;
#endif
            }
        }


        private static string WindDegree2Direction(float windDeg)
        {
            string[] dirs = new string[] {
                 Application.Context.GetString(Resource.String.wind_dir_N),
                 Application.Context.GetString(Resource.String.wind_dir_NNE),
                 Application.Context.GetString(Resource.String.wind_dir_NE),
                 Application.Context.GetString(Resource.String.wind_dir_ENE),
                 Application.Context.GetString(Resource.String.wind_dir_E),
                 Application.Context.GetString(Resource.String.wind_dir_ESE),
                 Application.Context.GetString(Resource.String.wind_dir_SE),
                 Application.Context.GetString(Resource.String.wind_dir_SSE),
                 Application.Context.GetString(Resource.String.wind_dir_S),
                 Application.Context.GetString(Resource.String.wind_dir_SSW),
                 Application.Context.GetString(Resource.String.wind_dir_SW),
                 Application.Context.GetString(Resource.String.wind_dir_WSW),
                 Application.Context.GetString(Resource.String.wind_dir_W),
                 Application.Context.GetString(Resource.String.wind_dir_WNW),
                 Application.Context.GetString(Resource.String.wind_dir_NW),
                 Application.Context.GetString(Resource.String.wind_dir_NNW)};
            //windDeg = (float)Math.Round((windDeg - 11.5f)/22.5f);
            windDeg = (float)Math.Round((windDeg * 10 % 3600) / 225);
            return dirs[(int)windDeg];
        }


        private static async Task<Location> GetCurrentLocation()
        {
            var lastLoc = await Geolocation.GetLastKnownLocationAsync();
            Console.WriteLine("LAST LOC: " + lastLoc.ToString());
            var request = new GeolocationRequest();
            request.DesiredAccuracy = GeolocationAccuracy.Medium;
            request.Timeout = TimeSpan.FromSeconds(10);
            var currLoc = await Geolocation.GetLocationAsync(request);
            Console.WriteLine("LOC: " + currLoc.ToString());
            if (currLoc == null) return lastLoc;
            return currLoc;

        }
    }
}