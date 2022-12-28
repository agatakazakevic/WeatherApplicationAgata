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
using Android.Graphics;

namespace weatherapplication
{
    class WeatherInfo
    {
        public string TempUnit { get; set; }
        public string SpeedUnit { get; set; }
        public string CityName { get; set; }
        public float Lat { get; set; }
        public float Long { get; set; }
        public float Currenttemp { get; set; }
        public float MinTemp { get; set; }
        public float Maxtemp { get; set; }
        public float FeelsLikeTemp { get; set; }
        public float Windspeed { get; set; }
        public float WindDeg { get; set; }
        public byte Humidity { get; set; }
        public int Pressure { get; set; }
        public string Description { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public ImageView photo { get; set; }
        public Bitmap Icon { get; set; }
        public string Country { get; internal set; }
        public string WindDir { get; internal set; }

    }
}