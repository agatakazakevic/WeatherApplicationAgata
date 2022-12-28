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

        public static async Task<WeatherInfo> GetCurrentWeatherData()
        {
            try
            {
                string key = "566630dc8b634a403e4e0ddaf9e66564";
                string city_name = "Vilnius";
                UnitsOfMeasurement units = UnitsOfMeasurement.metric;

                string source = $"http://api.openweathermap.org/data/2.5/weather?q={city_name}&appid={key}&units={units}";

                var data = await HttpHelper.GetDataFromAPI(source);
                if (data == null) return null;
                //try parsing data
                WeatherInfo info = new WeatherInfo();
                info.TempUnit = GetTempUnitValue(units);
                info.SpeedUnit = GetSpeedUnitValue(units);

                info.Lat = (float)data["coord"]["lat"];
                info.Long = (float)data["coord"]["lon"];
                info.CityName = data["name"].ToString();
                info.Descr = data["weather"][0]["description"].ToString();
                info.CurrentTemp = (float)data["main"]["temp"];
                info.MaxTemp = (float)data["main"]["temp_max"];
                info.MinTemp = (float)data["main"]["temp_min"];
                info.FeelsLikeTemp = (float)data["main"]["feels_like"];
                info.Humidity = (byte)data["main"]["humidity"];
                info.Pressure = (int)data["main"]["pressure"];
                info.WindSpeed = (float)data["wind"]["speed"];
                info.WindDeg = (float)data["wind"]["deg"];
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
#endif
            }
        }
    }
}