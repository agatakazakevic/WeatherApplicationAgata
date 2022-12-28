using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace weatherapplication.Fragments
{
    public class fragmentone : Android.Support.V4.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        string thisStatus;
        public fragmentone(string status)
        {
            thisStatus = status;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.progress, container, false);
            TextView statusTextView = (TextView)view.FindViewById(Resource.Id.progressStatus);
            statusTextView.Text = thisStatus;
            return view;
        }
    }
}