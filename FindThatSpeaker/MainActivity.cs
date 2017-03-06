using System;
using System.IO;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Media;
using System.Collections;
using FFTLibrary;
using System.Numerics;
using Java.Lang;

namespace FindThatSpeaker
{
    [Activity(Label = "FindThatSpeaker", MainLauncher = true, Icon = "@drawable/appicon")]
    public class MainActivity : Activity
    {
        string filePath = "";
        string filePath2 = "";
        string filename = "testAudio.wav";
        string filename2 = "testAudio2.wav";

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

            //filePath2 = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic) + "/" + filename2;

            log = FindViewById<TextView>(Resource.Id.log);
            log.Text = "-Log-\n\nwaiting...";

            startRecordButton.Click += StartRecord;
            //TestFFT();
        }

        private void StartRecord(object sender, System.EventArgs e)
        {
            startRecordButton.Enabled = false;//disable button
           // startRecordButton.Text = "recording";

            log.Text = "-Log-\n\nrecording...";//update the log

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

                h.PostDelayed(myAction, 3000);//run the action defined above after 5 seconds
            }
            catch (System.Exception ex)
            {
                Console.Out.WriteLine(ex.StackTrace);
            }            

        }
        
        private void StopRecord(object sender, System.EventArgs e)
        {
            log.Text = "-Log-\n\nwaiting...";//update the log
           // startRecordButton.Text = "Tap to record";
            recorder.Stop();
            recorder.Reset();
            recorder.Release();
            recorder = null;
            startRecordButton.Enabled = true;//re-enable button
            TestFFT();
        }

        private byte[] GetFileBytes(string filename)
        {
            byte[] bytes;
            using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
            }
            /*BitArray bits = new BitArray(bytes);
            for (int i = 0; i < bits.Count; i++)
            {
                bool bit = bits.Get(i);
                Console.Write(bit ? 1 : 0);
            }*/
            return bytes;
        }

        private void TestFFT()
        {
            byte[] data = GetFileBytes(filePath);
           
            double[] prex = new double[data.Length];//real array before fft performed
            double[] prey = new double[data.Length];//imaginary array before fft performed
            double[] postx = new double[data.Length];//real array after fft performed
            double[] posty = new double[data.Length];//imaginary array after fft performed

            //cls.StripSilence(filePath);

            //converting byte array into double array
            for (int i = 0; i < data.Length; i++)
             {
                prex[i] = data[i] / 32768.0;
                prey[i] = 0;
                postx[i] = data[i] / 32768.0;
                posty[i] = 0;

            }

            Fft.Transform(postx, posty);//perform fourier fast transform   
            
            string path = "test.csv";
            var documentsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic);
            var destinationPath = Path.Combine(documentsPath.ToString(), path);


            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            
            //var csv = new StringBuilder();

            using (var w = new StreamWriter(destinationPath))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    //display contents of input --> output arrays
                    Console.WriteLine("(" + (prex[i]) + "," + (prey[i]) + ")-->(" + (postx[i]) + "," + (posty[i]) + ")");

                    //create CSV file
                    var first = prex[i].ToString();
                    var second = prey[i].ToString();
                    var third = postx[i].ToString();
                    var fourth = posty[i].ToString();
                    //Suggestion made by KyleMit
                    var newLine = string.Format("{0},{1},{2},{3}", first, second, third, fourth);
                    // csv.Append(newLine);
                    w.WriteLine(newLine);
                    w.Flush();
                }
            }
            //File.WriteAllText(destinationPath, csv.ToString());
        }
    }
}