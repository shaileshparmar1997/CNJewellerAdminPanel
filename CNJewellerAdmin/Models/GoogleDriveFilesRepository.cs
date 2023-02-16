using CNJewellerAdmin.DTOs.GoogleDrive;
using CNJewellerAdmin.Helper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting;

namespace CNJewellerAdmin.Models
{
    public class GoogleDriveFilesRepository
    {
        public string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };

        private readonly IWebHostEnvironment _hostEnvironment;
        public GoogleDriveFilesRepository(IWebHostEnvironment hostEnvironment)
        {
            this._hostEnvironment = hostEnvironment;
        }
        //create Drive API service.
        public Google.Apis.Drive.v3.DriveService GetService()
        {
            //get Credentials from client_secret.json file 
            UserCredential credential;
            string CSPath = Path.Combine(_hostEnvironment.WebRootPath, "Content");

            using (var stream = new FileStream(Path.Combine(CSPath, "client_secret.json"), FileMode.Open, FileAccess.Read))
            {
                String FolderPath = Path.Combine(_hostEnvironment.WebRootPath, "Content");
                String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }

            //create Drive API service.
            Google.Apis.Drive.v3.DriveService service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveRestAPI-v3"
            });
            return service;
        }

        public Google.Apis.Drive.v2.DriveService GetService_v2()
        {
            UserCredential credential;
            var CSPath = Path.Combine(_hostEnvironment.WebRootPath, "Content");

            using (var stream = new FileStream(Path.Combine(CSPath, "client_secret.json"), FileMode.Open, FileAccess.Read))
            {

                String FilePath = Path.Combine(CSPath, "DriveServiceCredentials.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }

            //Create Drive API service.
            Google.Apis.Drive.v2.DriveService service = new Google.Apis.Drive.v2.DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveRestAPI-v2",
            });
            return service;
        }

        //get all files from Google Drive.
        public List<GoogleDriveFileNew> GetDriveFiles()
        {
            Google.Apis.Drive.v3.DriveService service = GetService();

            // Define parameters of request.
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
            // for getting folders only.
           // FileListRequest.Q = "mimeType='application/vnd.google-apps.folder'";
            FileListRequest.Fields = "nextPageToken, files(*)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFileNew> FileList = new List<GoogleDriveFileNew>();


            // For getting only folders
            // files = files.Where(x => x.MimeType == "application/vnd.google-apps.folder").ToList();


            if (files != null && files.Count > 0)
            {
                foreach (var file in files.OrderBy(x => x.MimeType))
                {
                    GoogleDriveFileNew File = new GoogleDriveFileNew
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime,
                        Parents = file.Parents,
                        MimeType = file.MimeType,
                        ThumbnailLink = file.ThumbnailLink,
                        FileExtensions = file.FileExtension,
                        WebContentLink = file.WebContentLink,
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }

        //file Upload to the Google Drive root folder.
        //public   void FileUpload(IFormFile file)
        //{
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        Google.Apis.Drive.v3.DriveService service = GetService();

        //        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/GoogleDriveFiles"),
        //        Path.GetFileName(file.FileName));
        //        file.SaveAs(path);

        //        var FileMetaData = new Google.Apis.Drive.v3.Data.File();
        //        FileMetaData.Name = Path.GetFileName(file.FileName);
        //        FileMetaData.MimeType = MimeMapping.GetMimeMapping(path);

        //        Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request;

        //        using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
        //        {
        //            request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
        //            request.Fields = "id";
        //            request.Upload();
        //        }


        //        // Create Folder in Drive


        //    }
        //}

        public void FileUpload(IFormFile file)
        {
            if (file != null)
            {
                Google.Apis.Drive.v3.DriveService service = GetService();
                string path = Path.Combine(_hostEnvironment.WebRootPath, "GoogleDriveFiles", Path.GetFileName(file.FileName));

                //string path = Path.Combine(HttpContext.Current.Server.MapPath("~/GoogleDriveFiles"),
                Path.GetFileName(file.FileName);
                System.IO.File.Create(path);
                //file.SaveAs(path);

                var FileMetaData = new Google.Apis.Drive.v3.Data.File();
                FileMetaData.Name = Path.GetFileName(file.FileName);
                FileMetaData.MimeType = MimeHelper.GetMimeMapping(path);

                Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request;

                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }
                // Create Folder in Drive    
            }
        }

        //Download file from Google Drive by fileId.
        public string DownloadGoogleFile(string fileId)
        {
            Google.Apis.Drive.v3.DriveService service = GetService();

            string FolderPath = Path.Combine(_hostEnvironment.WebRootPath, "GoogleDriveFiles");
            Google.Apis.Drive.v3.FilesResource.GetRequest request = service.Files.Get(fileId);

            string FileName = request.Execute().Name;
            string FilePath = System.IO.Path.Combine(FolderPath, FileName);

            MemoryStream stream1 = new MemoryStream();

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream1, FilePath);
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream1);
            return FilePath;
        }

        // file save to server path
        private void SaveStream(MemoryStream stream, string FilePath)
        {
            using (System.IO.FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }

        //Delete file from the Google drive
        public void DeleteFile(GoogleDriveFileNew files)
        {
            Google.Apis.Drive.v3.DriveService service = GetService();
            try
            {
                // Initial validation.
                if (service == null)
                    throw new ArgumentNullException("service");

                if (files == null)
                    throw new ArgumentNullException(files.Id);

                // Make the request.
                service.Files.Delete(files.Id).Execute();
            }
            catch (Exception ex)
            {
                throw new Exception("Request Files.Delete failed.", ex);
            }
        }

        public List<GoogleDriveFileNew> GetContainsInFolder(String folderId)
        {
            List<string> ChildList = new List<string>();
            Google.Apis.Drive.v2.DriveService ServiceV2 = GetService_v2();
            ChildrenResource.ListRequest ChildrenIDsRequest = ServiceV2.Children.List(folderId);

            // for getting only folders
            //ChildrenIDsRequest.Q = "mimeType='application/vnd.google-apps.folder'";
            do
            {
                var children = ChildrenIDsRequest.Execute();

                if (children.Items != null && children.Items.Count > 0)
                {
                    foreach (var file in children.Items)
                    {
                        ChildList.Add(file.Id);
                    }
                }
                ChildrenIDsRequest.PageToken = children.NextPageToken;

            } while (!String.IsNullOrEmpty(ChildrenIDsRequest.PageToken));

            //Get All File List
            List<GoogleDriveFileNew> AllFileList = GetDriveFiles();
            List<GoogleDriveFileNew> Filter_FileList = new List<GoogleDriveFileNew>();

            foreach (string Id in ChildList)
            {
                Filter_FileList.Add(AllFileList.Where(x => x.Id == Id).FirstOrDefault());
            }
            return Filter_FileList;
        }

        // Create Folder in root
        public void CreateFolder(string FolderName)
        {
            Google.Apis.Drive.v3.DriveService service = GetService();

            var FileMetaData = new Google.Apis.Drive.v3.Data.File();
            FileMetaData.Name = FolderName;
            FileMetaData.MimeType = "application/vnd.google-apps.folder";

            Google.Apis.Drive.v3.FilesResource.CreateRequest request;

            request = service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = request.Execute();
            Console.WriteLine("Folder ID: " + file.Id);
        }

        // Create multiple folder in drive
        //public List<FolderListDTO> CreateDriveFolder(string FolderName)
        //{
        //    Google.Apis.Drive.v3.DriveService service = GetService();

        //    var FileMetaData = new Google.Apis.Drive.v3.Data.File();
        //    FileMetaData.Name = FolderName;
        //    FileMetaData.MimeType = "application/vnd.google-apps.folder";

        //    Google.Apis.Drive.v3.FilesResource.CreateRequest request;

        //    request = service.Files.Create(FileMetaData);
        //    request.Fields = "id";
        //    var file = request.Execute();
        //    List<FolderListDTO> abc = new List<FolderListDTO>();

        //    return file.Id;
        //}

        // Create Folder in existing folder
        public void CreateFolderInFolder(string folderId, string FolderName)
        {

            Google.Apis.Drive.v3.DriveService service = GetService();

            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(FolderName),
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>
                    {
                        folderId
                    }
            };


            Google.Apis.Drive.v3.FilesResource.CreateRequest request;

            request = service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = request.Execute();
            Console.WriteLine("Folder ID: " + file.Id);

            var file1 = request;

        }

        // File upload in existing folder
        //public   void FileUploadInFolder(string folderId, HttpPostedFileBase file)
        //{
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        Google.Apis.Drive.v3.DriveService service = GetService();

        //        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/GoogleDriveFiles"),
        //        Path.GetFileName(file.FileName));
        //        file.SaveAs(path);

        //        var FileMetaData = new Google.Apis.Drive.v3.Data.File()
        //        {
        //            Name = Path.GetFileName(file.FileName),
        //            MimeType = MimeMapping.GetMimeMapping(path),
        //            Parents = new List<string>
        //            {
        //                folderId
        //            }
        //        };

        //        Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request;
        //        using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
        //        {
        //            request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
        //            request.Fields = "id";
        //            request.Upload();
        //        }
        //        var file1 = request.ResponseBody;
        //    }
        //}


        // check Folder name exist or note in root
        public bool CheckFolder(string FolderName)
        {
            bool IsExist = false;

            Google.Apis.Drive.v3.DriveService service = GetService();

            // Define parameters of request.
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
            FileListRequest.Fields = "nextPageToken, files(*)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFileNew> FileList = new List<GoogleDriveFileNew>();


            //For getting only folders
            files = files.Where(x => x.MimeType == "application/vnd.google-apps.folder" && x.Name == FolderName).ToList();

            if (files.Count > 0)
            {
                IsExist = false;
            }
            return IsExist;
        }


        public List<GoogleDriveFileNew> GetDriveFolders()
        {
            Google.Apis.Drive.v3.DriveService service = GetService();
            List<GoogleDriveFileNew> FolderList = new List<GoogleDriveFileNew>();

            Google.Apis.Drive.v3.FilesResource.ListRequest request = service.Files.List();
            request.Q = "mimeType='application/vnd.google-apps.folder'";
            //request.Fields = "parents";
           request.Fields = "files(*)";

            Google.Apis.Drive.v3.Data.FileList result = request.Execute();
            //result.Files = result.Files.Where(x => x.Parents == null).ToList();
            foreach (var file in result.Files)
            {
                GoogleDriveFileNew File = new GoogleDriveFileNew
                {
                    Id = file.Id,
                    Name = file.Name,
                    Size = file.Size,
                    Version = file.Version,
                    CreatedTime = file.CreatedTime
                };
                FolderList.Add(File);
            }
            return FolderList;
        }

        public string MoveFiles(String fileId, String folderId)
        {
            Google.Apis.Drive.v3.DriveService service = GetService();

            // Retrieve the existing parents to remove
            Google.Apis.Drive.v3.FilesResource.GetRequest getRequest = service.Files.Get(fileId);
            getRequest.Fields = "parents";
            Google.Apis.Drive.v3.Data.File file = getRequest.Execute();
            string previousParents = String.Join(",", file.Parents);

            // Move the file to the new folder
            Google.Apis.Drive.v3.FilesResource.UpdateRequest updateRequest = service.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId);
            updateRequest.Fields = "id, parents";
            updateRequest.AddParents = folderId;
            updateRequest.RemoveParents = previousParents;

            file = updateRequest.Execute();
            if (file != null)
            {
                return "Success";
            }
            else
            {
                return "Fail";
            }
        }
        public string CopyFiles(String fileId, String folderId)
        {
            Google.Apis.Drive.v3.DriveService service = GetService();

            // Retrieve the existing parents to remove
            Google.Apis.Drive.v3.FilesResource.GetRequest getRequest = service.Files.Get(fileId);
            getRequest.Fields = "parents";
            Google.Apis.Drive.v3.Data.File file = getRequest.Execute();

            // Copy the file to the new folder
            Google.Apis.Drive.v3.FilesResource.UpdateRequest updateRequest = service.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId);
            updateRequest.Fields = "id, parents";
            updateRequest.AddParents = folderId;
            //updateRequest.RemoveParents = previousParents;
            file = updateRequest.Execute();
            if (file != null)
            {
                return "Success";
            }
            else
            {
                return "Fail";
            }
        }

        private void RenameFile(String fileId, String newTitle)
        {
            try
            {
                Google.Apis.Drive.v2.DriveService service = GetService_v2();

                Google.Apis.Drive.v2.Data.File file = new Google.Apis.Drive.v2.Data.File();
                file.Title = newTitle;

                // Rename the file.
                Google.Apis.Drive.v2.FilesResource.PatchRequest request = service.Files.Patch(file, fileId);
                Google.Apis.Drive.v2.Data.File updatedFile = request.Execute();

                //return updatedFile;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                //return null;
            }
        }

        internal List<GoogleDriveFileNew> GetContainsInFolder(List<CreateSharedDataRequest> sharedItems)
        {
            throw new NotImplementedException();
        }
    }
}
