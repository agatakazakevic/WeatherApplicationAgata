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
namespace weatherapplication
{
    [Activity(Label = "SettingActivity")]
    public class SettingActivity : PreferenceActivity
    {
        Preference citypref;
        Preference unitpref;
        Preference languagepref;
        Preference locpref;
     

        
        void initobjects()
        {
            citypref = PreferenceManager.FindPreference(GetString(Resource.String.city_pref_key));
            citypref.PreferenceChange += Allprefs_PreferenceChange;
            unitpref = PreferenceManager.FindPreference(GetString(Resource.String.unit_pref_key));
            unitpref.PreferenceChange += Allprefs_PreferenceChange;

            languagepref = PreferenceManager.FindPreference(GetString(Resource.String.language_pref_key));
            languagepref.PreferenceChange += Allprefs_PreferenceChange;

            locpref = PreferenceManager.FindPreference(GetString(Resource.String.loc_pref_key));

            locpref.PreferenceChange += Allprefs_PreferenceChange;
            displaysummary();
            apphelper.languagechanged = false;
            apphelper.prefchanged = false;
        }

        private void Allprefs_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            if (e.Preference.Key == locpref.Key)
            {
                var editor1 = apphelper.sharedpref.Edit();
                editor1.PutBoolean(e.Preference.Key, (bool)e.NewValue);
                editor1.Commit();
            }
            else
            {
                //get current value
                string val = apphelper.sharedpref.GetString(e.Preference.Key, "");
                if (val == e.NewValue.ToString())
                {

                    return;
                }
                Console.WriteLine("=========change=============");

                var editor = apphelper.sharedpref.Edit();
                editor.PutString(e.Preference.Key, e.NewValue.ToString());
                editor.Commit();
                if (e.Preference.Key==languagepref.Key)
                {
                    apphelper.languagechanged = true;
                    apphelper.changelanguage(this);
                    this.Recreate();
                }
                displaysummary();
            }
            apphelper.prefchanged = true;
        }

        private void displaysummary()
        {

           citypref.Title= GetString(Resource.String.city_pref_title)+apphelper.sharedpref.GetString(citypref.Key, GetString(Resource.String.defaultcity));
            //unit
           var unit_array= Resources.GetTextArray(Resource.Array.unit_array);
           var unit_array_values= Resources.GetTextArray(Resource.Array.unit_array_values);
            var unit=apphelper.sharedpref.GetString(unitpref.Key, GetString(Resource.String.defaultunit));
            for (int i = 0; i < unit_array.Length; i++)
            {
                if (unit_array_values[i]==unit)
                {
                    unitpref.Title = GetString(Resource.String.unit_pref_title) + unit_array[i];
                    break;
                }
            }
            //language
            var lang_array = Resources.GetTextArray(Resource.Array.lang_array);
            var lang_array_values = Resources.GetTextArray(Resource.Array.lang_array_values);
            var lang =apphelper.sharedpref.GetString(languagepref.Key, GetString(Resource.String.defaultlanguage));
            for (int i = 0; i < lang_array.Length; i++)
            {
                if (lang_array_values[i] == lang)
                {
                    languagepref.Title= GetString(Resource.String.language_pref_title)+lang_array[i];
                    break;
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            AddPreferencesFromResource(Resource.Layout.Settinglayout);

            initobjects();
        }
    }
}