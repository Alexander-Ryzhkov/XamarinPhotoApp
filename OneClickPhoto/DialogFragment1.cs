using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.IO;

namespace OneClickPhoto
{
    public class DialogFragment1 : DialogFragment
    {
        public String folderPath = "OneClickPhoto";
        public string[] files;
        public string spyAppPath;
        public int currentIndex;
        public ListView listview;
        public Activity act;
        public TextAdapter adapter;

        public static DialogFragment1 NewInstance(Bundle bundle)
        {
            DialogFragment1 fragment = new DialogFragment1();
            fragment.Arguments = bundle;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            spyAppPath = System.IO.Path.Combine(sdCardPath, folderPath);
            if (!Directory.Exists(spyAppPath))
            {
                Directory.CreateDirectory(spyAppPath);
            }
            files = Directory.GetDirectories(spyAppPath);

            View view = inflater.Inflate(Resource.Layout.DialogItemLayout, container, false);
            Button CloseButton = view.FindViewById<Button>(Resource.Id.CloseButton);
            Button RenameButon = view.FindViewById<Button>(Resource.Id.RenameButton);
            EditText Name = view.FindViewById<EditText>(Resource.Id.editTextName);
            CloseButton.Click += delegate {
                Dismiss();
            };
            RenameButon.Click += delegate
            {
                if (Name.Text == "")
                {
                    Dismiss();
                    UpdateFiles();
                    listview.Adapter = new TextAdapter(act, files);
                }
                else
                {
                    try
                    {
                        files = Directory.GetDirectories(spyAppPath);
                        Directory.Move(files[currentIndex], spyAppPath + "/" + Name.Text);
                        Dismiss();
                        SessionsActivity s = (SessionsActivity)act;
                        s.UpdateSessionsAdapter();
                        adapter = new TextAdapter(act, files);
                        listview.Adapter = adapter;
                        adapter.NotifyDataSetChanged();
                    }
                    catch (Exception ex)
                    {
                        Dismiss();
                    }
                }
            };
            return view;
        }

        public void GetInfo(String folderPath, ref string[] files, string spyAppPath, int curentIndex, ref ListView listview, Activity act, ref TextAdapter adapter)
        {
            this.folderPath = folderPath;
            this.files = files;
            this.spyAppPath = spyAppPath;
            this.currentIndex = curentIndex;
            this.listview = listview;
            this.act = act;
            this.adapter = adapter;
        }

        public void UpdateFiles()
        {
            files = Directory.GetDirectories(spyAppPath);
        }
    }
}