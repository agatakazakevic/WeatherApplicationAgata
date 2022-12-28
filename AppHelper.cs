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
using Android.Preferences;
using Android.Content.Res;

namespace WeatherApplication
{
    class AppHelper
    {
        public static bool prefChanged;
        public static bool fistRun;
        public static bool langChanged;

        public static ISharedPreferences sharedPref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);

        private static string defaultLang;
        public static Configuration conf = new Configuration();
        public static Resources res = Application.Context.Resources;
        

        public static bool ChangeLanguage(Context context)
        {
            defaultLang = sharedPref.GetString(context.GetString(Resource.String.lang_pref_key), context.GetString(Resource.String.default_lang));
            //if (!langChanged) return false;
            res = context.Resources;
            conf = res.Configuration;
            conf.SetLocale(new Java.Util.Locale(defaultLang));
            res.UpdateConfiguration(conf, res.DisplayMetrics);
            return true;
            
        }
    }
}