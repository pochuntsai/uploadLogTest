using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using System.Threading;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace UploadLogTest
{
    [Activity(Label = "UploadLogTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string filePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                                               "Trilateration_Android");
        string tag = "Brian";
        string sas = "https://servicerobotlog.blob.core.windows.net/logs?sv=2015-04-05&sr=c&sig=ar8KiZwBkqfa5w4l%2FCWpDQ01Y8WD9LQbewQKXbSTgKc%3D&se=2016-07-23T03%3A29%3A45Z&sp=rwdl";
        int count = 1;
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
                       
            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += async delegate
            {
                button.Text = string.Format("{0} clicks!", count++);
                await UseContainerSAS(sas);
            };
        }

        async Task UseContainerSAS(string sas)
        {
            //Try performing container operations with the SAS provided.

            //Return a reference to the container using the SAS URI.
            CloudBlobContainer container = new CloudBlobContainer(new Uri(sas));
            string date = DateTime.Now.ToString();
            try
            {
                //Write operation: write a new blob to the container.
                CloudBlockBlob blob = container.GetBlockBlobReference("HelloWorld.png");
                filePath = System.IO.Path.Combine(filePath, "HelloWorld.png");
                var fileStream = System.IO.File.OpenRead(filePath);
                await blob.UploadFromStreamAsync(fileStream);
                Log.Debug(tag, "upload file successfully!!!");
            }
            catch (Exception e)
            {
               Log.Debug(tag, "upload file fail!!!");
               Log.Debug(tag, "Additional error information: " + e.Message);
               
            }
            /*
            try
            {
                //Read operation: Get a reference to one of the blobs in the container and read it.
                CloudBlockBlob blob = container.GetBlockBlobReference("sasblob_” + date + “.txt");
                string data = await blob.DownloadTextAsync();

                Console.WriteLine("Read operation succeeded for SAS " + sas);
                Console.WriteLine("Blob contents: " + data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine("Read operation failed for SAS " + sas);
                Console.WriteLine();
            }
            Console.WriteLine();
            */
            try
            {
                //Delete operation: Delete a blob in the container.
                CloudBlockBlob blob = container.GetBlockBlobReference("log_2016-04-08.txt");
                await blob.DeleteAsync();
                Log.Debug(tag, "Delete file successfully!!!");
                Console.WriteLine("Delete operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Log.Debug(tag, "delete file fail!!!");
                Log.Debug(tag, "Additional error information: " + e.Message);
            }
        }
        
        private void uploadLog()
        {
            //if (checkLogFileExit())
            {
               
                                               
                /* WebClient way
                Uri uploadUri = new Uri(uploadUrl);
                WebClient myWebClient = new WebClient();
                myWebClient.UploadFileAsync(uploadUri, filePath);
                myWebClient.UploadFileCompleted += (object sender, UploadFileCompletedEventArgs e) =>
                {
                    //Show complete message
                    Toast.MakeText(this, "Log file is upload to server successfully!!!", ToastLength.Long);
                };
                */ 
            }
        }
        

        private bool checkLogFileExit()
        {
            string fileName = "log" + DateTime.Today.AddDays(-1).ToString("yyyyMMdd") + "txt";
            filePath = System.IO.Path.Combine(filePath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                Log.Debug(tag, "Log file is exist!!");
                return true;
            }
            else 
            {
                Log.Debug(tag, "Log file is NOT exist!!");
                return false;
            }
        }
    }
}

