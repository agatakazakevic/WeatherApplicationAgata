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
using Android.Content.Res;
using Android.Preferences;
namespace weatherapplication
{
    class apphelper
    {
        public static bool prefchanged;

        public static bool firstrun;
        public static ISharedPreferences sharedpref=PreferenceManager.GetDefaultSharedPreferences(Application.Context);

        private static string defaultlang;

        public static Configuration conf = new Configuration();

        public static Resources res = Application.Context.Resources;
        public static bool languagechanged;
        public static bool changelanguage(Context context)
        {
            defaultlang = sharedpref.GetString(context.GetString(Resource.String.language_pref_key), context.GetString(Resource.String.defaultlanguage));
            //if (!languagechanged)
            //{
            //    return false;
           // }
            conf = res.Configuration;
            res = context.Resources;
            conf.SetLocale(new Java.Util.Locale(defaultlang));
            res.UpdateConfiguration(conf, res.DisplayMetrics);
            return true;
            
        }

    }
}