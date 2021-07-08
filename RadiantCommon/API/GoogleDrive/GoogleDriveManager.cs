using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace Radiant.Common.API.GoogleDrive
{
    public static class GoogleDriveManager
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static string TryFetchDocumentContent(string aFileName)
        {
            //string[] scopes = new string[] { DriveService.Scope.Drive }; // Full access

            //var keyFilePath = @"c:\file.p12";    // Downloaded from https://console.developers.google.com
            //var serviceAccountEmail = "xx@developer.gserviceaccount.com";  // found https://console.developers.google.com

            ////loading the Key file
            //var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);
            //var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            //{
            //    Scopes = scopes
            //}.FromCertificate(certificate));
            ////https://stackoverflow.com/questions/38839668/service-account-authentication-to-upload-a-file-to-google-drive


            return null;
        }
    }
}
