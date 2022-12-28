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
using Android.Appwidget;

namespace weatherapplication
{
    [BroadcastReceiver(Label ="MyFirstWidget")]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate})]
    [MetaData("android.appwidget.provider", Resource="@xml/widgetprovider")]
    class widgetcore:AppWidgetProvider
    {
        public override async void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            try
            {
                var info = await APIhelper.GetCurrentWeatherData(string.Empty);
                RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.widgetlayout);
                views.SetTextViewText(Resource.Id.widgettemp, info.Currenttemp + info.TempUnit);
                views.SetTextViewText(Resource.Id.widgetrefresh, DateTime.Now.ToString());
                views.SetImageViewBitmap(Resource.Id.widgeticon, info.Icon);
                //refresh register button click
                var intent = new Intent(context, typeof(widgetcore));
                intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
                intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);
                var pending = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
                views.SetOnClickPendingIntent(Resource.Id.widgetrefreshbutton, pending);
                //end register
                appWidgetManager.UpdateAppWidget(appWidgetIds, views);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
               
            }
           // base.OnUpdate(context, appWidgetManager, appWidgetIds); 
        }

    }
}