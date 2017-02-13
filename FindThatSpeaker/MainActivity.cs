﻿using System;
using System.IO;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Media;

namespace FindThatSpeaker
{
    [Activity(Label = "FindThatSpeaker", MainLauncher = true, Icon = "@drawable/appicon")]
    public class MainActivity : Activity
    {
        string filePath = "";
        string filename = "testAudio.mp3";

        ImageButton startRecordButton;

        TextView log;

        protected MediaRecorder recorder = new MediaRecorder();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            startRecordButton = FindViewById<ImageButton>(Resource.Id.startRecordingButton);

            //Get the file path for the media file
            filePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic) + "/" + filename;

            log = FindViewById<TextView>(Resource.Id.log);
            log.Text = "-Log-\n\n-filepath-\n" + filePath + "\n\nwaiting...";

            startRecordButton.Click += StartRecord;
        }

        private void StartRecord(object sender, System.EventArgs e)
        {
            startRecordButton.Enabled = false;//disable button
           // startRecordButton.Text = "recording";

            log.Text = "-Log-\n\n-filepath-\n"+ filePath + "\n\nrecording...";//update the log

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
                recorder.Reset();
                recorder.SetAudioSource(AudioSource.Mic);
                recorder.SetOutputFormat(OutputFormat.ThreeGpp);
                recorder.SetAudioEncoder(AudioEncoder.AmrNb);
                //recorder.SetMaxDuration(10000);
                // Initialized state.
                recorder.SetOutputFile(filePath);
                // DataSourceConfigured state.
                recorder.Prepare(); // Prepared state
                recorder.Start(); // Recording state.

                //Automatically stop recording after a certain time
                Handler h = new Handler();
                Action myAction = () =>
                {
                    // your code that you want to run after delay here
                    StopRecord(sender, e);
                };

                h.PostDelayed(myAction, 5000);//run the action defined above after 5 seconds
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.StackTrace);
            }            

        }
        
        private void StopRecord(object sender, System.EventArgs e)
        {
            log.Text = "-Log-\n\n-filepath-\n" + filePath + "\n\nrecording stopped...";//update the log
           // startRecordButton.Text = "Tap to record";
            recorder.Stop();
            recorder.Reset();
            recorder.Release();
            recorder = null;
            startRecordButton.Enabled = true;//re-enable button
        }
        
    }
}