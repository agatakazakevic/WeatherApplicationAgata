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
using System.Globalization;
using System.Net;
using System.IO;
using Android.Graphics;
using Microcharts.Droid;
using Microcharts;
using SkiaSharp;
using Android.Support.V7.App;
using System.Net.Http;
using Newtonsoft.Json.Linq;


namespace WeatherApplication
{
    [Activity(Label = "chartactivity")]
    public class chartactivity : Activity
    {
        ChartView chartview;
        TextView pollutiondecr;
        long lan;
        long lat;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.chart);
            pollutiondecr = FindViewById<TextView>(Resource.Id.poluttiondescr);
            lat = Intent.GetLongExtra("lat", lat);
            lan = Intent.GetLongExtra("lan", lan);
            chartview = (ChartView)FindViewById(Resource.Id.chartView);
            GetWeather(lat, lan);
            
           

            // Create your application here
        }
        public override void OnBackPressed()
        {

            base.OnBackPressed();

        }
        async void GetWeather(long lat, long lan)
        {
              string apiKey = "adc8ea8fdb7174993372f26c0c82735a";
                string url = $"http://api.openweathermap.org/data/2.5/air_pollution/forecast?lat={lat}&lon={lan}&appid={apiKey}";

                var handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);
                string result = await client.GetStringAsync(url);

                Console.WriteLine(result);

                var resultObject = JObject.Parse(result);
                float indexco = (float)resultObject["list"][0]["components"]["co"];
                string airpollutionco = indexco.ToString();

                int indexdecr = (int)resultObject["list"][0]["main"]["aqi"];
            if (indexdecr == 1)
            {
                 pollutiondecr.Text = "Air quality:" + desccriptionofpollution.good + " " + indexdecr.ToString();
            }
            if (indexdecr == 2)
            {
                pollutiondecr.Text = "Air quality:" + desccriptionofpollution.fair + " " + indexdecr.ToString();
            }
            if (indexdecr == 3)
            {
                pollutiondecr.Text = "Air quality:" + desccriptionofpollution.moderate + " " + indexdecr.ToString();
            }
            if (indexdecr == 4)
            {
                pollutiondecr.Text = "Air quality:" + desccriptionofpollution.poor + " " + indexdecr.ToString();
            }
            if (indexdecr == 5)
            {
                pollutiondecr.Text = "Air quality:" + desccriptionofpollution.verypoor + " " + indexdecr.ToString();
            }



            float indexno = (float)resultObject["list"][1]["components"]["no"];
                string airpollutionno = indexno.ToString();

                float indexno2 = (float)resultObject["list"][2]["components"]["no2"];
                string airpollutionno2 = indexno2.ToString();

                float indexso2 = (float)resultObject["list"][4]["components"]["so2"];
                string airpollutionso2 = indexso2.ToString();
                List<Entry> DataList = new List<Entry>();

                DataList.Add(new Entry(indexco)
                {
                    Label = "CO",
                    ValueLabel =airpollutionco,
                    Color = SKColor.Parse("#266489")
                });

                DataList.Add(new Entry(indexno)
                {
                    Label = "NO",
                    ValueLabel = airpollutionno.ToString(),
                    Color = SKColor.Parse("#0ccf40")
                });

                DataList.Add(new Entry(indexno2)
                {
                    Label = "NO2",
                    ValueLabel = airpollutionno2,
                    Color = SKColor.Parse("#eb0e33")
                });

                DataList.Add(new Entry(indexso2)
                {
                    Label = "SO2",
                    ValueLabel = airpollutionso2,
                    Color = SKColor.Parse("#1068eb")
                });

                var chart = new PointChart() { Entries = DataList, LabelTextSize = 30f };
                chartview.Chart = chart;
            



            }
           
}




        }





