#pragma warning disable CS0618
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware;
using Android.Media;

using static Android.Views.View;
using static Android.Views.GestureDetector;

namespace OneClickPhoto
{
    [Activity(Label = "OneClickPhoto", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class CameraActivity : Activity, ISurfaceHolderCallback, Android.Hardware.Camera.IPictureCallback, IOnGestureListener, IOnTouchListener
    {
        private ImageAdapter sessionGridAdapter;
        private bool previewEnabled = false;
        private string sessionFolderPath;
        //private string appFolderName = "OneClickPhoto";
        private ISurfaceHolder cameraSurfaceHolder;
        private Android.Hardware.Camera deviceCamera;
        private GestureDetector gDetector;
        private MediaRecorder recorder;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CameraLayout);
            CreateCameraSurface();
            CreateNewSessionFolder();
            CreateSessionGridView();
            gDetector = new GestureDetector(this);
        }

        private void CreateCameraSurface()
        {
            SurfaceView cameraSurfaceView = FindViewById<SurfaceView>(Resource.Id.cameraSurfaceView);
            cameraSurfaceHolder = cameraSurfaceView.Holder;
            cameraSurfaceHolder.AddCallback(this);
            cameraSurfaceHolder.SetType(SurfaceType.PushBuffers);
        }

        private void CreateNewSessionFolder()
        {
            sessionFolderPath = Intent.GetStringExtra("sessionFolderPath");
            if (sessionFolderPath == null)
            {
                sessionFolderPath = System.IO.Path.Combine(FilesHandler.appFolderPath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(sessionFolderPath);
            }
        }

        private void CreateSessionGridView()
        {
            int displayWidthInDp = Resources.DisplayMetrics.WidthPixels;
            GridView sessionGridView = FindViewById<GridView>(Resource.Id.sessionGridView);
            sessionGridView.NumColumns = 1;
            sessionGridAdapter = new ImageAdapter(this, sessionFolderPath, displayWidthInDp);
            sessionGridView.Adapter = sessionGridAdapter;
            sessionGridView.SetOnTouchListener(this);
        }

        public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
        {
            if (previewEnabled)
            {
                deviceCamera.StopPreview();
                previewEnabled = false;
            }
            if (deviceCamera != null)
            {
                try
                {
                    deviceCamera.SetPreviewDisplay(cameraSurfaceHolder);
                    deviceCamera.StartPreview();
                    previewEnabled = true;
                }
                catch (Java.IO.IOException e)
                {
                    e.PrintStackTrace();
                }
            }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            OpenCamera();
        }

        private void OpenCamera()
        {
            deviceCamera?.Release();
            try
            {
                deviceCamera = Android.Hardware.Camera.Open();
            }
            catch (Exception e)
            {
                //Do nothing.
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            deviceCamera?.StopPreview();
            deviceCamera?.Release();
            deviceCamera = null;
            previewEnabled = false;
        }

        public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
        {
            //BitmapFactory.Options options = new BitmapFactory.Options();
            //options.InPurgeable = true;
            //Bitmap bitmapPicture = BitmapFactory.DecodeByteArray(data, 0, data.Length, options);
            //bitmapPicture = Bitmap.CreateScaledBitmap(bitmapPicture, bitmapPicture.Width / 1, bitmapPicture.Height / 1, false);
            Bitmap bitmapPicture = BitmapFactory.DecodeByteArray(data, 0, data.Length);
            var rotation = GetRotation();
            Matrix mtx = new Matrix();
            mtx.PreRotate(rotation);
            bitmapPicture = Bitmap.CreateBitmap(bitmapPicture, 0, 0, bitmapPicture.Width, bitmapPicture.Height, mtx, false);
            ExportBitmapAsJPG(bitmapPicture);
            //sessionGridAdapter.UpdateItemList();
        }

        private int GetRotation()
        {
            var rotation = base.WindowManager.DefaultDisplay.Rotation;
            switch (rotation)
            {
                case SurfaceOrientation.Rotation0:
                    return 90;
                case SurfaceOrientation.Rotation90:
                    return 0;
                case SurfaceOrientation.Rotation180:
                    return 270;
                case SurfaceOrientation.Rotation270:
                    return 180;
            }
            return 0;
        }

        void ExportBitmapAsJPG(Bitmap bitmap)
        {
            var filePath = System.IO.Path.Combine(sessionFolderPath, string.Format($"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_ffff")}.jpg"));
            var stream = new FileStream(filePath, FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 25, stream);
            stream.Close();
        }

        public bool OnDown(MotionEvent e)
        {
            return true;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return true;
        }

        public void OnLongPress(MotionEvent e)
        {
            recorder = new MediaRecorder();
            deviceCamera.StartPreview();
            deviceCamera.Unlock();
            recorder.SetCamera(deviceCamera);

            ////recorder.SetVideoSource(VideoSource.Camera);
            ////recorder.SetAudioSource(AudioSource.Camcorder);
            ////recorder.SetOutputFormat(OutputFormat.Mpeg4);
            recorder.SetVideoSource(VideoSource.Camera);
            recorder.SetAudioSource(AudioSource.Camcorder);
            recorder.SetOutputFormat(OutputFormat.Mpeg4);
            recorder.SetVideoEncoder(VideoEncoder.Default);
            recorder.SetAudioEncoder(AudioEncoder.Default);
            //recorder.SetVideoSize(1920, 1080);
            //recorder.SetVideoEncoder(VideoEncoder.H264);
            ////recorder.SetVideoEncoder(VideoEncoder.Mpeg4Sp);
            ////recorder.SetAudioEncoder(AudioEncoder.AmrNb);
            var rotation = GetRotation();
            recorder.SetOrientationHint(rotation);
            var videoPath = System.IO.Path.Combine(sessionFolderPath, string.Format($"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_ffff")}.mp4"));
            recorder.SetOutputFile(videoPath);
            recorder.SetPreviewDisplay(cameraSurfaceHolder.Surface);
            recorder.Prepare();
            recorder.Start();
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return true;
        }

        public void OnShowPress(MotionEvent e)
        {
            // Do nothing.
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            try
            {
                deviceCamera.StartPreview();
                deviceCamera.TakePicture(null, null, this);
                return true;
            }
            catch
            {
                return false;
            };
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (recorder != null && e.Action == MotionEventActions.Up)
            {
                try
                {
                    recorder.Stop();
                    Wait(500);
                    recorder.Reset();
                    recorder.Release();
                    //sessionGridAdapter.UpdateItemList();
                }
                catch { }
                finally
                {
                    recorder = null;
                }
            }
            gDetector.OnTouchEvent(e);
            return false;
        }

        protected override void OnResume()
        {
            base.OnResume();
            OpenCamera();
        }

        protected override void OnPause()
        {
            base.OnPause();
            releaseMediaRecorder();       // if you are using MediaRecorder, release it first
            releaseCamera();              // release the camera immediately on pause event
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            releaseMediaRecorder();      
            releaseCamera();              
        }

        private void releaseMediaRecorder()
        {
            if (recorder != null)
            {
                recorder.Reset();   // clear recorder configuration
                recorder.Release(); // release the recorder object
                recorder = null;
                deviceCamera.Lock() ;           // lock camera for later use
            }
        }

        private void releaseCamera()
        {
            if (deviceCamera != null)
            {
                deviceCamera.Release();        // release the camera for other applications
                deviceCamera = null;
            }
        }


    }
}