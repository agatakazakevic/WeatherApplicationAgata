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

namespace WeatherApplication
{
    class ForecastAdapter : BaseAdapter<List<WeatherInfo>>
    {
        Context context;
        List<WeatherInfo> infoLst;

        public ForecastAdapter(Context context, List<WeatherInfo> infoLst)
        {
            this.context = context;
            this.infoLst = infoLst;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            ForecastAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as ForecastAdapterViewHolder;

            if (holder == null)
            {
                holder = new ForecastAdapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.forecastLayout, parent, false);
                //layout elements
                holder.Temp = view.FindViewById<TextView>(Resource.Id.forecastTemp);
                holder.Descr = view.FindViewById<TextView>(Resource.Id.forecastDesr);
                holder.Icon = view.FindViewById<ImageView>(Resource.Id.forecastIcon);
                view.Tag = holder;
            }


            //fill in your items
            holder.Temp.Text = infoLst[position].CurrentTemp + infoLst[position].TempUnit;

            holder.Descr.Text = infoLst[position].Descr;
            holder.Icon.SetImageBitmap(infoLst[position].Icon);

            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return infoLst.Count;
            }
        }

        public override List<WeatherInfo> this[int position]
        {
            get 
            { 
                return infoLst; 
            }
        }
    }

    class ForecastAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        public TextView Temp { get; set; }
        public TextView Descr { get; set; }
        public ImageView Icon { get; set; }
    }
}