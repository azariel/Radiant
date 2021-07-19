﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using Radiant.Common.Diagnostics;
using Radiant.Common.Helpers;
using File = Google.Apis.Drive.v3.Data.File;

namespace Radiant.Common.API.GoogleDrive
{
    public static class GoogleDriveManager
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static DriveService AuthenticateServiceAccount()
        {
            const string _JSON_SERVICE_KEY_FILE_NAME = "API/GoogleDrive/radiant-319014-d7d51b9a40d1.json";

            try
            {
                if (string.IsNullOrEmpty(_JSON_SERVICE_KEY_FILE_NAME))
                    throw new Exception("Path to the .JSon service account credentials file is required.");
                if (!System.IO.File.Exists(_JSON_SERVICE_KEY_FILE_NAME))
                    throw new Exception("The service account credentials .JSon file does not exist at:" + _JSON_SERVICE_KEY_FILE_NAME);

                // These are the scopes of permissions you need. It is best to request only what you need and not all of them
                string[] scopes =
                {
                    DriveService.Scope.Drive,// View and manage the files in your Google Drive
                    DriveService.Scope.DriveAppdata,// View and manage its own configuration data in your Google Drive
                    DriveService.Scope.DriveFile,// View and manage Google Drive files and folders that you have opened or created with this app
                    DriveService.Scope.DriveMetadata,// View and manage metadata of files in your Google Drive
                    //DriveService.Scope.DriveMetadataReadonly,// View metadata for files in your Google Drive
                    //DriveService.Scope.DrivePhotosReadonly,// View the photos, videos and albums in your Google Photos
                    //DriveService.Scope.DriveReadonly,// View the files in your Google Drive
                    DriveService.Scope.DriveScripts
                };// Modify your Google Apps Script scripts' behavior
                Stream stream = new FileStream(_JSON_SERVICE_KEY_FILE_NAME, FileMode.Open, FileAccess.Read, FileShare.Read);
                var credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);

                // Create the  Drive service.
                return new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Drive Authentication Sample"
                });
            } catch (Exception ex)
            {
                //Console.WriteLine("Create service account DriveService failed" + ex.Message);
                throw new Exception("CreateServiceAccountDriveServiceFailed", ex);
            }
        }

        ///// <summary>
        ///// Update file containing version
        ///// </summary>
        ///// <returns></returns>
        //private static bool UpdateVersionFile()
        //{
        //    Assembly _Assembly = Assembly.GetExecutingAssembly();
        //    FileVersionInfo _FileVersionInfo = FileVersionInfo.GetVersionInfo(_Assembly.Location);
        //    return CreateOrUpdateFile(FILE_VERSION_FILE_ID, _FileVersionInfo.FileVersion);
        //}

        public static bool CreateOrUpdateFile(string aFileId, string aFileContent)
        {
            File _File = TryGetFile(aFileId);

            if (_File != null)
            {
                TryUpdateFileContent(aFileId, aFileContent);
                return true;
            }

            TryCreateNewFile(aFileId, aFileContent, aFileId);
            return true;
        }

        public static bool DeleteFiles(string[] aFileIds)
        {
            try
            {
                DriveService _DriveService = AuthenticateServiceAccount();
                foreach (string _FileId in aFileIds)
                    _DriveService.Files.Delete(_FileId).Execute();
            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("2B6A1983-0923-4D7F-858A-9A4DC1325BF5", $"Couldn't delete document content. FileIDs were [{string.Join(",", aFileIds)}].", _Exception);
                return false;
            }

            return true;
        }

        public static string GenerateFileId()
        {
            try
            {
                DriveService _DriveService = AuthenticateServiceAccount();
                FilesResource.GenerateIdsRequest _GeneratedIdsRequest = _DriveService.Files.GenerateIds();
                _GeneratedIdsRequest.Count = 1;
                GeneratedIds _GeneratedIds = _GeneratedIdsRequest.Execute();

                return _GeneratedIds.Ids.Single();
            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("DE6FBEF8-E72E-4A16-9312-912434A9E4A3", "Couldn't generate new file id from google service.", _Exception);
                return null;
            }
        }

        public static bool PruneEverything()
        {
            try
            {
                DriveService _DriveService = AuthenticateServiceAccount();
                FileList _FilesList = _DriveService.Files.List().Execute();
                DeleteFiles(_FilesList.Files.Select(s => s.Id).ToArray());

                // Delete folder by folder
                var _FoldersList = _DriveService.Drives.List().Execute();
                DeleteFiles(_FoldersList.Drives.Select(s => s.Id).ToArray());
            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("ECC062C5-DC1A-40B8-8D1E-17A6B032979E", "Couldn't prune everything.", _Exception);
                return false;
            }

            return true;
        }

        public static string TryCreateNewFile(string aFileNameWithExtension, string aFileContent, string aFileId = null)
        {
            using var _MemoryStream = aFileContent.ToStream();
            try
            {
                DriveService _DriveService = AuthenticateServiceAccount();

                var _FileMetadata = new File
                {
                    Name = aFileNameWithExtension
                };

                if (aFileId != null)
                    _FileMetadata.Id = aFileId;

                FilesResource.CreateMediaUpload _Request = _DriveService.Files.Create(_FileMetadata, _MemoryStream, "text/plain");
                _Request.Fields = "id";
                IUploadProgress _Response = _Request.Upload();

                if (_Response.Status == UploadStatus.Failed)
                {
                    LoggingManager.LogToFile("E5F155EC-4C4F-43B0-A190-B68C101E2B7B", $"Couldn't create new document. Request failed. Filename was [{aFileNameWithExtension}].");
                    return null;
                }

                File _File = _Request.ResponseBody;

                return _File.Id;
            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("FFEAF804-B0F8-425B-A910-7139A3AC3915", $"Couldn't create new document. Filename was [{aFileNameWithExtension}].", _Exception);
                return null;
            }
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static string TryFetchDocumentContentAsString(string aFileID)
        {
            try
            {
                DriveService _DriveService = AuthenticateServiceAccount();
                FilesResource.GetRequest _FileRequest = _DriveService.Files.Get(aFileID);

                // Download file content to memory stream
                using MemoryStream _MemoryStream = new MemoryStream();
                _FileRequest.Download(_MemoryStream);

                string _StringContent = Encoding.ASCII.GetString(_MemoryStream.ToArray());
                return _StringContent;
            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("3EF01799-16E1-48F8-9E24-7FA5DE6C731A", $"Couldn't fetch document content as string. FileID was [{aFileID}].", _Exception);
                return null;
            }
        }

        public static File TryGetFile(string aFileId)
        {
            try
            {
                DriveService _DriveService = AuthenticateServiceAccount();
                FilesResource.GetRequest _FileRequest = _DriveService.Files.Get(aFileId);
                return _FileRequest.Execute();
            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("1CD25C90-37DA-4B2F-97E3-FE6F9F998354", $"Couldn't fetch document. FileID was [{aFileId}].", _Exception);
            }

            return null;
        }

        public static void TryUpdateFileContent(string aFileId, string aFileContent)
        {
            //if (aFile == null)
            //    aFile = TryGetFile(aFileId);

            Stream _Stream = aFileContent.ToStream();
            try
            {
                DriveService _DriveService = AuthenticateServiceAccount();
                var _MediaUpload = _DriveService.Files.Update(null, aFileId, _Stream, "text/plain").Upload();

                if (_MediaUpload.Status == UploadStatus.Failed)
                {
                    LoggingManager.LogToFile("2BA3A24F-2EEC-4666-A130-30EAA0A312BB", $"Couldn't update document content. Request failed. FileID was [{aFileId}].");
                }
            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("F253BAFE-30A4-4DB7-8A85-22D3F643C7B6", $"Couldn't update document content. FileID was [{aFileId}].", _Exception);
            } finally
            {
                _Stream.Close();
            }
        }

        //public static bool TryDeleteFile(string aFileID)
        //{
        //    try
        //    {
        //        DriveService _DriveService = AuthenticateServiceAccount();
        //        FilesResource.UpdateMediaUpload _Response = _DriveService.Files.Update(null, aFileID, aFileContent.ToStream(), "text/plain");

        //    } catch (Exception _Exception)
        //    {
        //        LoggingManager.LogToFile("F253BAFE-30A4-4DB7-8A85-22D3F643C7B6", $"Couldn't update document content. FileID was [{aFileID}].", _Exception);
        //    }
        //}
    }
}
