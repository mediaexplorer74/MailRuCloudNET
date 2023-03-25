using MailRuCloudClient;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        public const string Login = "********@mail.ru";
        public const string Password = "*******************";

        private static Account account = null;
        private static CloudClient client = null;

        private const string TestFolderName = "new folder"; // In Cloud
        private const string TestFolderPath = "/";// + TestFolderName; // In Cloud
        private const string TestFolderPublicLink = "https://cloud.mail.ru/public/JWXJ/xsyPB2eZU"; // In Cloud
        private const string TestFileName = @"video.mp4"; // The common file name
        private const string TestUploadFilePath = @"C:\Users\Erast\Downloads\" + TestFileName; // On local machine
        private const string TestDownloadFilePath = TestFolderPath + "/" + TestFileName; // In Cloud
        private const string TestHistoryCheckingFilePath = "/Новая таблица.xlsx"; // In Cloud, this file need to create manually and fill history

        private int prevUploadProgressPercentage = -1;
        private int prevDownloadProgressPercentage = -1;
        private bool hasChangedFolderContentAfterUploading = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            CreateFolderTest();
        }

        static public async Task CreateFolderTest()
        {
            await CheckAuthorization();
            var newFolderName = /*Guid.NewGuid().ToString()*/"newfoldertest";
            var result = await client.CreateFolder(TestFolderPath 
                /*+ "/new folders test/"*/ + newFolderName);

            string res = "";
            try
            {
                res = result.FullPath.Split(new[] { '/' }).Last();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Debug.WriteLine(newFolderName +" | "+ res );
            Debug.WriteLine(TestFolderPath + "newfoldertest | " +  result.FullPath);
        }

        static private async Task CheckAuthorization()
        {
            if (account == null)
            {
                account = new Account(Login, Password);
                bool outp = /*await*/ account.Login();

                Debug.WriteLine(outp == true ? "true":"false");

                client = new CloudClient(account);
            }
        }
    }
}
