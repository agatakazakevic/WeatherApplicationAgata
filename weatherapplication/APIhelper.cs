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

namespace weatherapplication
{
    class APIhelper
    {
        private static string gettempunitvalue(UnitOfMeasurement units)
        {
            switch (units)
            {
                case UnitOfMeasurement.standart:
                    return "K";
                   
                case UnitOfMeasurement.metric:
                    return "°C ";
                case UnitOfMeasurement.imperial:
                    return "°F ";
            }
            return "";
        }
        private static string  getspeedunitvalue(UnitOfMeasurement units)
        {
            switch (units)
            {
                case UnitOfMeasurement.standart:
                case UnitOfMeasurement.metric:
                    return "m/s";
                case UnitOfMeasurement.imperial:
                    return "mi/h";
              
            }
            return " ";
        }
        public static async Task<WeatherInfo> GetCurrentWeatherData(string searchcity)
        {
            try
            {
                string key = "e0256c00571c0d96921868047631bcdd";
                string city_name = apphelper.sharedpref.GetString(Application.Context.GetString(Resource.String.city_pref_key), Application.Context.GetString(Resource.String.defaultcity));
                if (!string.IsNullOrEmpty(searchcity))
                {
                    city_name = searchcity;
                }
                string unit_name = apphelper.sharedpref.GetString(Application.Context.GetString(Resource.String.unit_pref_key), Application.Context.GetString(Resource.String.defaultunit));
                UnitOfMeasurement units = (UnitOfMeasurement)(Enum.Parse(typeof(UnitOfMeasurement), unit_name));
               string lang_name= apphelper.sharedpref.GetString(Application.Context.GetString(Resource.String.language_pref_key), Application.Context.GetString(Resource.String.defaultlanguage));

                bool useloc = apphelper.sharedpref.GetBoolean(Application.Context.GetString(Resource.String.loc_pref_key), false);

                Location loc = new Location();

                string source = $"http://api.openweathermap.org/data/2.5/weather?q={city_name}&appid={key}&units={units}&lang={lang_name}";
                if (useloc && string.IsNullOrEmpty(searchcity))
                {
                    Task.Run(async delegate
                    {
                        loc=await getcurrentlocation();
                    }).Wait(); //ne lokali kopija, o tik adresa paduodame
                    source = $"http://api.openweathermap.org/data/2.5/weather?lat={loc.Latitude}&lon={loc.Longitude}&appid={key}&units={units}&lang={lang_name}";
                }

                var data = await httphelper.GetDataFromAPI(source);
                if (data==null)
                {
                    return null;
                }
                //try parsing data
                WeatherInfo info = new WeatherInfo();
                info.TempUnit = gettempunitvalue(units);
                info.SpeedUnit = getspeedunitvalue(units);
                info.Lat = (float)data["coord"]["lat"];
                info.Long = (float)data["coord"]["lon"];
                info.CityName = data["name"].ToString();
                info.Country = data["sys"]["country"].ToString();
                info.Description= data["weather"][0]["description"].ToString();
                info.Currenttemp = (float)data["main"]["temp"];
                info.Maxtemp = (float)data["main"]["temp_max"];
                info.MinTemp = (float)data["main"]["temp_min"];
                info.FeelsLikeTemp = (float)data["main"]["feels_like"];
                info.Humidity = (byte)data["main"]["humidity"];
                info.Pressure = (int)data["main"]["pressure"];
                info.Windspeed = (float)data["wind"]["speed"];
                info.WindDeg = (float)data["wind"]["deg"];
                info.WindDir = WindDegree2Direction(info.WindDeg);
                int timezone = (int)data["timezone"];
                info.Sunrise = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds((double)data["sys"]["sunrise"]).AddSeconds(timezone);
                info.Sunset = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds((double)data["sys"]["sunset"]).AddSeconds(timezone);
             
                //get picture
                Task.Run(async delegate
                {
                    info.Icon = await httphelper.getweathericon(data["weather"][0]["icon"].ToString());
                }).Wait();
               



                return info;
            }
            catch (Exception ex)
            {

#if DEBUG
                Toast.MakeText(Application.Context, "Data parsing error:" + ex.Message, ToastLength.Short).Show();
                Console.WriteLine("==========================");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("==========================");
                return null;
#else
Toast.MakeText(Application.Context, "Cannot parse data:" + ex.Message, ToastLength.Short).Show();
                return null;
#endif
            }
        }

  

        public static async Task<List<WeatherInfo>> GetForecastWeatherData(string searchcity)
        {
            try
            {
                string key = "e0256c00571c0d96921868047631bcdd";
                string city_name = apphelper.sharedpref.GetString(Application.Context.GetString(Resource.String.city_pref_key), Application.Context.GetString(Resource.String.defaultcity));
                if (!string.IsNullOrEmpty(searchcity))
                {
                    city_name = searchcity;
                }
                string unit_name = apphelper.sharedpref.GetString(Application.Context.GetString(Resource.String.unit_pref_key), Application.Context.GetString(Resource.String.defaultunit));
                UnitOfMeasurement units = (UnitOfMeasurement)(Enum.Parse(typeof(UnitOfMeasurement), unit_name));
                string lang_name = apphelper.sharedpref.GetString(Application.Context.GetString(Resource.String.language_pref_key), Application.Context.GetString(Resource.String.defaultlanguage));

                bool useloc = apphelper.sharedpref.GetBoolean(Application.Context.GetString(Resource.String.loc_pref_key), false);

                Location loc = new Location();

                string source = $"http://api.openweathermap.org/data/2.5/forecast?q={city_name}&appid={key}&units={units}&lang={lang_name}";
                if (useloc && string.IsNullOrEmpty(searchcity))
                {
                    Task.Run(async delegate
                    {
                        loc = await getcurrentlocation();
                    }).Wait(); //ne lokali kopija, o tik adresa paduodame
                    source = $"http://api.openweathermap.org/data/2.5/forecast?lat={loc.Latitude}&lon={loc.Longitude}&appid={key}&units={units}&lang={lang_name}";
                }

                var data = await httphelper.GetDataFromAPI(source);
                if (data == null)
                {
                    return null;
                }
                //try parsing data
                List<WeatherInfo> infolst = new List<WeatherInfo>();
                var cnt = (int)data["cnt"];
                for (int i = 0; i < cnt; i++)
                {
                    WeatherInfo info = new WeatherInfo();

                    info.TempUnit = gettempunitvalue(units);
                    info.Description = data["list"][i]["dt_txt"]+"\n"+data["list"][i]["weather"][0]["description"].ToString();
                    info.Currenttemp = (float)data["list"][i]["main"]["temp"];
                    //get picture
                    Task.Run(async delegate
                    {
                        info.Icon = await httphelper.getweathericon(data["list"][i]["weather"][0]["icon"].ToString());
                    }).Wait();


                    infolst.Add(info);
                }
                return infolst;
            }
            catch (Exception ex)
            {

#if DEBUG
                Toast.MakeText(Application.Context, "Data parsing error:" + ex.Message, ToastLength.Short).Show();
                Console.WriteLine("==========================");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("==========================");
                return null;
#else
Toast.MakeText(Application.Context, "Cannot parse data:" + ex.Message, ToastLength.Short).Show();
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
            windDeg = (float)Math.Round((windDeg - 11.5f)/22.5f);
            //windDeg = (float)Math.Round((windDeg * 10 % 3600) / 225);
            return dirs[(int)windDeg];
        }
        private static async Task<Location> getcurrentlocation()
        {
           var lastloc = await Geolocation.GetLastKnownLocationAsync();
            var request = new GeolocationRequest();
           request.DesiredAccuracy=GeolocationAccuracy.Low;
            request.Timeout = TimeSpan.FromSeconds(10);
            var curloc = await Geolocation.GetLocationAsync(request);
            if (curloc==null)
            {
                return lastloc;
            }
            return curloc;
        }
    }
}