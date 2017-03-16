extern alias flexitools;

using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using flexitools::ABBYY.FlexiCapture;

namespace FlexiTools
{
    public class Processor
    {

        public IProcessingCallback Processing;
        public Receipt receipt;

        public Processor(string ApplicationId, string password, IProcessingCallback proc)
        {

            this.Processing = proc;
            Processing.ReportMessage("Starting Receipt processing!");

            restClient = new RestServiceClient();
            restClient.Proxy.Credentials = CredentialCache.DefaultCredentials;

            //!!! Please provide your application id and password in Config.txt
            // To create an application and obtain a password,
            // register at http://cloud.ocrsdk.com/Account/Register
            // More info on getting your application id and password at
            // http://ocrsdk.com/documentation/faq/#faq3

            // Name of application you created
            restClient.ApplicationId = ApplicationId;
            // Password should be sent to your e-mail after application was created
            restClient.Password = password;

            // Display hint to provide credentials
            if (String.IsNullOrEmpty(restClient.ApplicationId) ||
                String.IsNullOrEmpty(restClient.Password))
            {
                Processing.ReportError("Please provide access credentials to Cloud OCR SDK service!");
                throw new Exception("Please provide access credentials to Cloud OCR SDK service!");
            }

            Console.WriteLine(String.Format("Application id: {0}\n", restClient.ApplicationId));
        }

        public delegate void StepChangedActionHandler( string description );
        public StepChangedActionHandler StepChangedAction;

        public delegate void ProgressChangedActionHandler( int progress );
        public ProgressChangedActionHandler ProgressChangedAction;

        public Receipt Process( string imagePath, ProcessingSettings settings )
        {
            string result = null;

            Console.WriteLine("Uploading image..." );
            if(this.Processing != null)
            {
                Processing.ReportMessage("Uploading image...");
            }
            

            OcrSdkTask task = restClient.ProcessImage(imagePath, settings);

            Console.WriteLine("Processing...");

            if (this.Processing != null)
            {
                Processing.ReportMessage("Processing...");
            }

            task = waitForTask(task);

            if (task.Status == TaskStatus.Completed)
            {
                Console.WriteLine("Processing completed.");
                Processing.ReportMessage("Processing completed.");

                result = restClient.DownloadUrl(task.DownloadUrls[0]);

                Console.WriteLine("Download completed.");
                Processing.ReportMessage("Download completed.");
            }
            else if (task.Status == TaskStatus.NotEnoughCredits)
            {
                Processing.ReportMessage("Not enough credits to process the file. Please add more pages to your application balance.");
                throw new Exception("Not enough credits to process the file. Please add more pages to your application balance.");
            }
            else
            {
                Processing.ReportMessage("Error while processing the task");
                throw new Exception("Error while processing the task");
            }

            receipt = new Receipt(result, this.Processing);

            return receipt;


        }

        private string getValueByKey(string line, string key)
        {
            string[] keyValue = line.Split( '=' );
            string value = "";
            if(keyValue.Length == 2 ) {
                value = keyValue[1].Trim();
            }
            return value;
        }

        private OcrSdkTask waitForTask(OcrSdkTask task)
        {
            Console.WriteLine(String.Format("Task status: {0}", task.Status));
            Processing.ReportMessage(String.Format("Task status: {0}", task.Status));

            while (task.IsTaskActive())
            {
                // Note: it's recommended that your application waits
                // at least 2 seconds before making the first getTaskStatus request
                // and also between such requests for the same task.
                // Making requests more often will not improve your application performance.
                // Note: if your application queues several files and waits for them
                // it's recommended that you use listFinishedTasks instead (which is described
                // at http://ocrsdk.com/documentation/apireference/listFinishedTasks/).
                System.Threading.Thread.Sleep(5000);
                task = restClient.GetTaskStatus(task.Id);

                Console.WriteLine(String.Format("Task status: {0}", task.Status));
                Processing.ReportMessage(String.Format("Task status: {0}", task.Status));
            }
            return task;
        }

        private RestServiceClient restClient;
    }
}
