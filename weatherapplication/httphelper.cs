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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Android.Graphics;
namespace weatherapplication
{
    class httphelper
    {
        public static async Task<JContainer> GetDataFromAPI(string source)
        {
            try
            {
                HttpClient client = new HttpClient();
                var data=await client.GetStringAsync(source);//galima parasyti wait(sekund.)
                if (string.IsNullOrWhiteSpace(data))
                {
                    return null;
                }
                return JsonConvert.DeserializeObject<JContainer>(data);
            }
            catch (Exception ex)
            {

#if DEBUG
                Toast.MakeText(Application.Context, "HTTP error" + ex.Message, ToastLength.Short).Show();
                Console.WriteLine(ex.StackTrace);
                return null;
#else
Toast.MakeText(Application.Context, "HTTP error" + ex.Message, ToastLength.Short).Show();
                return null;
#endif
            }
        }
        public static async Task< Bitmap> getweathericon(string iconname)
        {
            string iconpath = $"http://openweathermap.org/img/w/{iconname}.png";
            Java.Net.URL url = new Java.Net.URL(iconpath);
            return  await BitmapFactory.DecodeStreamAsync(url.OpenStream());
        }
    }
}