//using System;
using System.IO;
//using System.Collections;
//using System.Collections.Generic;

using Android.App;
using Android.Content;
//using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace OneClickPhoto
{
    [Activity(Label = "OneClickPhoto", MainLauncher = true, Icon = "@drawable/icon")]
    public class SessionsActivity : Activity
    {
        public string sessionFolderPath = "";
        public ListView sessionsListView;
        public TextAdapter sessionsListViewAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SessionsLayout);
            InitializeFilesHadler();
            InitializeSessions();
        }

        public void InitializeFilesHadler()
        {
            FilesHandler.UpdateAppFolderPath();
            FilesHandler.UpdateAppDirectories();
        }

        public void InitializeSessions()
        {
            InitializeAddNewSessionButton();
            InitializeSessionsListView();
            sessionsListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                SessionsListViewOnClickEvent(args);
            };
            //sessionsListView.ItemLongClick += delegate (object sender, AdapterView.ItemLongClickEventArgs args)
            //{
            //    SessionsListViewOnLongClickEvent(args);
            //};
            RegisterForContextMenu(sessionsListView);
        }

        public void InitializeAddNewSessionButton()
        {
            Button addNewSessionButton = FindViewById<Button>(Resource.Id.AddButton);
            addNewSessionButton.Click += delegate
            {
                StartActivity(typeof(CameraActivity));
            };
        }

        public void InitializeSessionsListView()
        {
            sessionsListView = FindViewById<ListView>(Resource.Id.SessionsList);
            sessionsListViewAdapter = new TextAdapter(this, FilesHandler.appDirectories);
            sessionsListView.Adapter = sessionsListViewAdapter;
        }

        public void SessionsListViewOnClickEvent(AdapterView.ItemClickEventArgs args)
        {
            sessionFolderPath = FilesHandler.appDirectories[args.Position];
            UpdateSessionsAdapter();
            var cameraActivity = new Intent(this, typeof(CameraActivity));
            cameraActivity.PutExtra("sessionFolderPath", sessionFolderPath);
            StartActivity(cameraActivity);
        }

        //public void SessionsListViewOnLongClickEvent(AdapterView.ItemLongClickEventArgs args)
        //{
        //    Directory.Delete(FilesHandler.appDirectories[args.Position], true);
        //    UpdateSessionsAdapter();
        //}

        public void UpdateSessionsAdapter()
        {
            FilesHandler.UpdateAppDirectories();
            sessionsListViewAdapter = new TextAdapter(this, FilesHandler.appDirectories);
            sessionsListView.Adapter = sessionsListViewAdapter;
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            UpdateSessionsAdapter();
        }

        public string cutPath(string s)
        {
            return Path.GetFileName(s);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            if (v.Id == Resource.Id.SessionsList)
            {
                var info = (AdapterView.AdapterContextMenuInfo)menuInfo;
                menu.SetHeaderTitle(cutPath(FilesHandler.appDirectories[info.Position]));
                var menuItems = Resources.GetStringArray(Resource.Array.menu);
                for (var i = 0; i < menuItems.Length; i++)
                    menu.Add(Menu.None, i, i, menuItems[i]);
            }
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            var menuItemIndex = item.ItemId;
            var menuItems = Resources.GetStringArray(Resource.Array.menu);
            var menuItemName = menuItems[menuItemIndex];
            var listItemName = FilesHandler.appDirectories[info.Position];

            if (menuItemName == "Delete")
                Directory.Delete(FilesHandler.appDirectories[info.Position], true);
            if (menuItemName == "Continue")
                Directory.Delete(FilesHandler.appDirectories[info.Position], true);
            if (menuItemName == "Rename")
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();

                Fragment prev = FragmentManager.FindFragmentByTag("dialog");
                if (prev != null)
                {
                    ft.Remove(prev);
                    FilesHandler.appDirectories = Directory.GetDirectories(FilesHandler.appFolderPath);
                    sessionsListViewAdapter = new TextAdapter(this, FilesHandler.appDirectories);
                    sessionsListView.Adapter = sessionsListViewAdapter;
                    sessionsListViewAdapter.NotifyDataSetChanged();
                }
                ft.AddToBackStack(null);

                DialogFragment1 newFragment = DialogFragment1.NewInstance(null);

                newFragment.GetInfo(FilesHandler.appFolderName, ref FilesHandler.appDirectories, FilesHandler.appFolderPath, info.Position, ref sessionsListView, (SessionsActivity)this, ref sessionsListViewAdapter);
                newFragment.Show(ft, "dialog");
                FilesHandler.appDirectories = Directory.GetDirectories(FilesHandler.appFolderPath);
            }
            return true;
        }
    }
}