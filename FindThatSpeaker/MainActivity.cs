using System;
using System.IO;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Media;


namespace FindThatSpeaker
{
    [Activity(Label = "FindThatSpeaker", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string filePath;
        string filename = "testAudio.mp3";
        Button startRecordButton;
        Button stopRecordButton;
        TextView log;

        public MediaRecorder recorder = new MediaRecorder();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            startRecordButton = FindViewById<Button>(Resource.Id.startRecordingButton);
            stopRecordButton = FindViewById<Button>(Resource.Id.stopRecordingButton);
            log = FindViewById<TextView>(Resource.Id.log);

            //Get the file path for the media file
            filePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic) + "/" + filename;

            startRecordButton.Click += StartRecord;
            stopRecordButton.Click += StopRecord;
        }

        private void StartRecord(object sender, System.EventArgs e)
        {
            log.Text = "recording...\n"+filePath;
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                if (recorder == null)
                {
                    recorder = new MediaRecorder(); // Initial state.
                }
                else
                {
                    recorder.Reset();
                    recorder.SetAudioSource(AudioSource.Mic);
                    recorder.SetOutputFormat(OutputFormat.ThreeGpp);
                    recorder.SetAudioEncoder(AudioEncoder.AmrNb);
                    // Initialized state.
                    recorder.SetOutputFile(filePath);
                    // DataSourceConfigured state.
                    recorder.Prepare(); // Prepared state
                    recorder.Start(); // Recording state.
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.StackTrace);
            }
        }

        private void StopRecord(object sender, System.EventArgs e)
        {
            log.Text = "recording stopped...";
            recorder.Stop();
            recorder.Reset();
            recorder.Release();
        }
    }
}

