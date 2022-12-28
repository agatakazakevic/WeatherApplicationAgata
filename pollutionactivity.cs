using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Net.Http;
using System;
using Newtonsoft.Json.Linq;




namespace WeatherApplication
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class pollutionactivity : AppCompatActivity
    {
      
        TextView pollutionco;
        TextView pollutiondecr;
        TextView pollutionno;
        TextView pollutionno2;
        TextView pollutionso2;
        
  
        string city;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.pollution);
            pollutionco = FindViewById<TextView>(Resource.Id.txtpolutionco);
            pollutiondecr = FindViewById<TextView>(Resource.Id.poluttiondescr);
            pollutionno = FindViewById<TextView>(Resource.Id.txtpolutionno);
            pollutionno2 = FindViewById<TextView>(Resource.Id.txtpolutionno2);
            pollutionso2 = FindViewById<TextView>(Resource.Id.txtpolutionso2);
            city = Intent.GetStringExtra("DATA_PASS");
        
            GetWeather(city);
           
    }

        public override void OnBackPressed()
        {
           
            base.OnBackPressed();
            Finish();
           
        }
        async void GetWeather(string place)
        {
            try
            {
                string apiKey = "adc8ea8fdb7174993372f26c0c82735a";
                string url = $"http://api.openweathermap.org/data/2.5/air_pollution/forecast?lat=50&lon=50&appid={apiKey}";
                
                var handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);
                string result = await client.GetStringAsync(url);

                Console.WriteLine(result);

                var resultObject = JObject.Parse(result);
                float indexco = (float)resultObject["list"][0]["components"]["co"];
                string airpollutionco = indexco.ToString();

                int indexdecr = (int)resultObject["list"][0]["main"]["aqi"];
                if (indexdecr==1)
                {
                    pollutiondecr.Text ="Air quality:"+desccriptionofpollution.good +" "+ indexdecr.ToString();
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

               
           
                pollutionno.Text = "NO concentration: "+airpollutionno+ "μg/m3";
                pollutionno2.Text = "NO2 concentration: "+airpollutionno2+ "μg/m3";
                pollutionco.Text = "CO concentration: "+airpollutionco+ "μg/m3";
                pollutionso2.Text = "SO2 concentration: "+airpollutionso2+ "μg/m3";
                
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("==============");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("==============");
            }
           

            
         
        }


     
    }
}