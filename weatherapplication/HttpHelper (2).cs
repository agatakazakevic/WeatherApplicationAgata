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
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Android.Graphics;

namespace WeatherApplication
{
    class HttpHelper
    {
        public static async Task<JContainer> GetDataFromAPI(string source)
        {
            try
            {
                HttpClient client = new HttpClient();
                var data =await client.GetStringAsync(source);

                if (string.IsNullOrWhiteSpace(data)) return null;

                client.Dispose();
                return JsonConvert.DeserializeObject<JContainer>(data);
               
            }
            catch (Exception ex)
            {
#if DEBUG
                Toast.MakeText(Application.Context, "HTTP Error:" + ex.Message, ToastLength.Long).Show();
                Console.WriteLine("=====================");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("=====================");
                return null;
#else
Toast.MakeText(Application.Context, "Something went wrong", ToastLength.Long).Show();
#endif
            }
        }

        public static async Task<Bitmap> GetWeatherIcon(string iconName)
        {
            string iconPath = $"http://openweathermap.org/img/w/{iconName}.png";
            Java.Net.URL url = new Java.Net.URL(iconPath);
            return await BitmapFactory.DecodeStreamAsync(url.OpenStream());
        }


    }
}