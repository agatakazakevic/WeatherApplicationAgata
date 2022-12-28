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

namespace WeatherApplication
{
    [BroadcastReceiver(Label ="MyFirstWidget")]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate})]
    [MetaData("android.appwidget.provider", Resource ="@xml/widgetprovider")]
    class WidgetCore:AppWidgetProvider
    {
        public override async void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            try
            {
                var info = await ApiHelper.GetCurrentWeatherData(string.Empty);
                RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.widgetLayout);
                views.SetTextViewText(Resource.Id.widgetTemp, info.CurrentTemp + info.TempUnit);
                views.SetTextViewText(Resource.Id.widgetRefresh, DateTime.Now.ToString());
                views.SetImageViewBitmap(Resource.Id.widgetIcon, info.Icon);
                //=====register refresh button click========
                var intent = new Intent(context, typeof(WidgetCore));
                intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
                intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);
                var pending = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
                views.SetOnClickPendingIntent(Resource.Id.widgetRefreshButton, pending);
                //=======end register=========

                appWidgetManager.UpdateAppWidget(appWidgetIds, views);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            //base.OnUpdate(context, appWidgetManager, appWidgetIds);
        }
    }
}