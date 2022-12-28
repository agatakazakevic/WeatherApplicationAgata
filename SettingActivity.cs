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

namespace WeatherApplication
{
    [Activity(Label = "SettingActivity")]
    public class SettingActivity : PreferenceActivity
    {
        Preference cityPref;
        Preference unitPref;
        Preference langPref;
        Preference locPref;

        void InitObjects()
        {
            cityPref = PreferenceManager.FindPreference(GetString(Resource.String.city_pref_key));
            cityPref.PreferenceChange += AllPrefs_PreferenceChange;

            unitPref = PreferenceManager.FindPreference(GetString(Resource.String.unit_pref_key));
            unitPref.PreferenceChange += AllPrefs_PreferenceChange;

            langPref = PreferenceManager.FindPreference(GetString(Resource.String.lang_pref_key));
            langPref.PreferenceChange += AllPrefs_PreferenceChange;

            locPref = PreferenceManager.FindPreference(GetString(Resource.String.loc_pref_key));
            locPref.PreferenceChange += AllPrefs_PreferenceChange;

            DisplaySummary();
            AppHelper.prefChanged = false;
            AppHelper.langChanged = false;
        }

        private void AllPrefs_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            if (e.Preference.Key == locPref.Key)
            {
                var editor1 = AppHelper.sharedPref.Edit();
                editor1.PutBoolean(e.Preference.Key, (bool)e.NewValue);
                editor1.Commit();
            }
            else
            {
                //get current value
                string val = AppHelper.sharedPref.GetString(e.Preference.Key, "");
                if (val == e.NewValue.ToString()) return;
                var editor = AppHelper.sharedPref.Edit();
                editor.PutString(e.Preference.Key, e.NewValue.ToString());
                editor.Commit();
                if (e.Preference.Key == langPref.Key)
                {
                    AppHelper.langChanged = true;
                    AppHelper.ChangeLanguage(this);
                    this.Recreate();
                }
                   
                DisplaySummary();
            }
            AppHelper.prefChanged = true;
            Toast.MakeText(this, AppHelper.prefChanged.ToString(), ToastLength.Short).Show();
        }

        private void DisplaySummary()
        {

            cityPref.Title = GetString(Resource.String.city_pref_title) + AppHelper.sharedPref.GetString(cityPref.Key, GetString(Resource.String.default_city));
            //unit
            var unit_array = Resources.GetTextArray(Resource.Array.unit_array);
            var unit_array_values = Resources.GetTextArray(Resource.Array.unit_array_values);
            var unit = AppHelper.sharedPref.GetString(unitPref.Key, GetString(Resource.String.default_unit));

            for (int i = 0; i < unit_array_values.Length; i++)
            {
                if (unit_array_values[i] == unit)
                {
                    unitPref.Title = GetString(Resource.String.unit_pref_title) + unit_array[i];
                    break;
                }
            }

            //language
            var lang_array = Resources.GetTextArray(Resource.Array.lang_array);
            var lang_array_values = Resources.GetTextArray(Resource.Array.lang_array_values);
            var lang = AppHelper.sharedPref.GetString(langPref.Key, GetString(Resource.String.default_lang));

            for (int i = 0; i < lang_array_values.Length; i++)
            {
                if (lang_array_values[i] == lang)
                {
                    langPref.Title = GetString(Resource.String.lang_pref_title) + lang_array[i];
                    break;
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            AddPreferencesFromResource(Resource.Layout.SettingsLayout);
            InitObjects();
        }
    }
}