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
using Android.Graphics;

namespace OneClickPhoto
{
    class ImageAdapter : BaseAdapter
    {
        int displayWidthInDp;
        string sessionFolderPath = "";
        string[] sessionFiles;
        Context context;

        public ImageAdapter(Context context, string sessionFolderPath, int displayWidthInDp)
        {
            this.context = context;
            this.sessionFolderPath = sessionFolderPath;
            this.displayWidthInDp = displayWidthInDp;
            this.sessionFiles = Directory.GetFiles(sessionFolderPath + "/");
        }

        public override int Count
        {
            get
            {
                return sessionFiles.Length;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            int reversedFilePosition = Count - position - 1;
            string fileExtension = System.IO.Path.GetExtension(sessionFiles[reversedFilePosition]);
            if (fileExtension == ".jpg")
                return GetImageView(reversedFilePosition);
            else if (fileExtension == ".mp4")
                return GetVideoView(reversedFilePosition);
            return convertView;
        }

        private ImageView GetImageView(int reversedFilePosition)
        {
            ImageView imageView = new ImageView(context);
            imageView.LayoutParameters = new GridView.LayoutParams(displayWidthInDp, displayWidthInDp);
            imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            imageView.SetPadding(8, 8, 8, 8);
            imageView.SetImageBitmap(BitmapFactory.DecodeFile(sessionFiles[reversedFilePosition]));
            return imageView;
        }

        private VideoView GetVideoView(int reversedFilePosition)
        {
            VideoView videoView = new VideoView(context); 
            videoView.LayoutParameters = new GridView.LayoutParams(displayWidthInDp, displayWidthInDp, GetItemViewType(reversedFilePosition));
            videoView.SetPadding(8, 8, 8, 8);
            videoView.SetVideoPath(sessionFiles[reversedFilePosition]);
            videoView.Start();
            var mc = new MediaController(context);
            mc.SetAnchorView(videoView);
            //mc.Layout(0, 0, 400, 0);
            mc.SetMediaPlayer(videoView);
            videoView.SetMediaController(mc);
            //var onClick = new OnClickListener();
            //videoView.SetOnClickListener(onClick);
            //videoView.SeekTo(1);
            //videoView.Touch += VideoClick;
            return videoView;
        }

        //private void VideoClick(object sender, Android.Views.View.TouchEventArgs e)
        //{
        //    if (e.Event.Action == MotionEventActions.Pointer2Down)
        //    {
        //        ((VideoView)sender).Start();
        //    }
        //}

        public void UpdateItemList()
        {
            sessionFiles = Directory.GetFiles(sessionFolderPath + "/");
            NotifyDataSetChanged();
        }

        public class OnClickListener : Java.Lang.Object, View.IOnClickListener
        {

            public void OnClick(View v)
            {
                if ( ((VideoView)v).IsPlaying )
                {
                    ((VideoView)v).Pause();
                }
                else
                {
                    ((VideoView)v).Start();
                }
            }

        }

    }
}