using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace WeatherApplication
{
    [Activity(Label = "savedcitiesactivity")]
    public class savedcitiesactivity : ListActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           // SetContentView(Resource.Layout.savedcitieslayout);
            //ListView lst = (ListView)FindViewById(Resource.Id.listView);

            //ArrayList list = (ArrayList)Intent.GetSerializableExtra("list");
            var obj = JsonConvert.DeserializeObject<ArrayList>(Intent.GetStringExtra("list"));
            // ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, listitems);

            ArrayAdapter ad = new ArrayAdapter(this, Resource.Layout.list_item, obj);
           

            ListView.SetAdapter(ad);

            ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                Toast.MakeText(Application, ((TextView)args.View).Text, ToastLength.Short).Show();
                string name= ((TextView)args.View).Text;
                MainActivity.selectedcity = name;
               // ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
               // ISharedPreferencesEditor editor = prefs.Edit();
               // editor.PutString("cityname", name);
                    
               // editor.Apply();
                Finish();
                
            };
        }

       
    }
}