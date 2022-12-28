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

namespace weatherapplication
{
    class forecastadapter : BaseAdapter<List<WeatherInfo>>
    {

        Context context;
        List<WeatherInfo> infolst;

        public forecastadapter(Context context, List<WeatherInfo> infolst)
        {
            this.context = context;
            this.infolst = infolst;
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
            forecastadapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as forecastadapterViewHolder;

            if (holder == null)
            {
                holder = new forecastadapterViewHolder();
                var inflater = context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.forecastlayout, parent, false);
                holder.Temp = view.FindViewById<TextView>(Resource.Id.forecasttemp);
                holder.Descr = view.FindViewById<TextView>(Resource.Id.forecastdescr);
                holder.Icon = view.FindViewById<ImageView>(Resource.Id.forecasticon);

                view.Tag = holder;
            }


            //fill in your items
            holder.Temp.Text = infolst[position].Currenttemp + infolst[position].TempUnit;
            holder.Descr.Text = infolst[position].Description;
            holder.Icon.SetImageBitmap(infolst[position].Icon);
            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return infolst.Count;
            }
        }

        public override List<WeatherInfo> this[int position]
        {
            get
            {
                return infolst;
            }
        }
    }

    class forecastadapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        public TextView Temp { get; set; }
        public TextView Descr { get; set; }
        public ImageView Icon { get; set; }
    }
}