using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OneClickPhoto
{
    public class TextAdapter : BaseAdapter<string>
    {
        string[] itemsInAdapter;
        string[] itemsInAdapterFilesNames;
        Activity context;

        public string GetFileNameFromPath(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public TextAdapter(Activity context, string[] itemsToPutInAddapter) : base()
        {
            this.context = context;
            this.itemsInAdapter = itemsToPutInAddapter;
        }

        public override string this[int position]
        {
            get
            {
                return GetFileNameFromPath(itemsInAdapter[position]);
            }
        }

        public override int Count
        {
            get
            {
                return itemsInAdapter.Length;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Array.Resize(ref itemsInAdapterFilesNames, itemsInAdapter.Length);
            for (int i = 0; i < itemsInAdapter.Length; i++)
                itemsInAdapterFilesNames[i] = GetFileNameFromPath(itemsInAdapter[i]);
            View view = convertView;
            if (view == null)
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = itemsInAdapterFilesNames[position];
            return view;
        }
    }
}