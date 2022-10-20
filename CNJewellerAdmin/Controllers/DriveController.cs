using CNJewellerAdmin.DTOs.GoogleDrive;
using CNJewellerAdmin.Helper.GDrive;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Drive.v3.DriveService;

namespace CNJewellerAdmin.Controllers
{
    public class DriveController : Controller
    {
        private readonly GDriveHelper _GDriveHelper;
        public string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };


        private readonly IWebHostEnvironment _hostEnvironment;
        public DriveController(IWebHostEnvironment hostEnvironment)
        {
            this._hostEnvironment = hostEnvironment;
            _GDriveHelper = new GDriveHelper(hostEnvironment);
        }

        [GoogleScopedAuthorize(DriveService.ScopeConstants.DriveReadonly)]
        public async Task<IActionResult> DriveFileList([FromServices] IGoogleAuthProvider auth)
        {
            GoogleCredential cred = await auth.GetCredentialAsync();
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
            var files = await service.Files.List().ExecuteAsync();
            var fileNames = files.Files.Select(x => x.Name).ToList();
            return View(fileNames);
        }

        public IActionResult Index()
        {
           // var abc = GetService();
            var driveFiles = GetFiles("temp");
            return View(driveFiles);
        }

        public IActionResult GetDriveData()
        {
            return View();
        }

        public Google.Apis.Drive.v3.DriveService GetService()
        {
            //get Credentials from client_secret.json file     
            UserCredential credential;
            //var CSPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/");
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
                ApplicationName = "GoogleDriveRestAPI-v3",
            });
            return service;
        }

        public IEnumerable<Google.Apis.Drive.v3.Data.File> GetFiles(string folder)
        {
            var service = GetService1();
            var fileList = service.Files.List();
            fileList.Q = $"mimeType!='application/vnd.google-apps.folder' and '{folder}' in parents";
            fileList.Fields = "nextPageToken, files(id, name, size, mimeType)";

            var result = new List<Google.Apis.Drive.v3.Data.File>();
            string pageToken = null;
            do
            {

                // IList<Google.Apis.Drive.v3.Data.File> files = fileList.Execute().Files;
                fileList.PageToken = pageToken;
                var filesResult = fileList.Execute();
                var files = filesResult.Files;
                pageToken = filesResult.NextPageToken;
                result.AddRange(files);
            }
            while (pageToken != null);
            return result;
        }


        private static DriveService GetService1()
        {
            var tokenResponse = new TokenResponse
            {
                AccessToken = "ya29.a0Aa4xrXP_5KdGC_rk9yGDZ0LgvMK1_2ij0sWLR0kSH5Rwl4AhvABbFwV3_eombAu56rXUx59YfKK2o944tJViyka_6n-A2H9Eo6ysQj3-H5ml6XHtmfoV0r6D5U2RuMLL_6FcoG2XKwGQ5Opn0IfvgZdOoVo8aCgYKATASARISFQEjDvL9vwm2K1g81Qpl0TPOZxlV4A0163",
                IdToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImVkMzZjMjU3YzQ3ZWJhYmI0N2I0NTY4MjhhODU4YWE1ZmNkYTEyZGQiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI0MDc0MDg3MTgxOTIuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI0MDc0MDg3MTgxOTIuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMDI5NzY4NzQxODM1MjE4NTk2ODIiLCJhdF9oYXNoIjoiN3AyRUljODZVT3U5ZTAtZjJucEt1dyIsImlhdCI6MTY2NTQ3MTU3MSwiZXhwIjoxNjY1NDc1MTcxfQ.faKyA_Ws3V1ne5Qnu1NKFJ2nHceupmoYarltMu5pT9SdcSPNgQEkbXcLBmmhJ9APewV5MOpl7dpq7M9ZI5U9U81ATnztPYvz0rT52SZM02-vftH6nPlgZ3Ko-6RBXBi_yL-I14Bz0TzFWcjWqSFm6pARMu6mGLyLrmC_UQ-F5A_U_Fd4eT7dxYgZii1dFNY6NBBQw6jebx28y8BbtW8hlzYUVLn59W8Ay0F0bAoHDK8gNct63z35OuOuJlVVJVJaHUZJxcZpACLEvB0MYd64KpSsP3-DH387mWj9s-uNB8HoDr0GgIPWWa8OfJkQt_ziokHHjsJMtdb9zkvrVB85wQ",
                Scope = "openid https://www.googleapis.com/auth/drive",
                TokenType= "Bearer",
                ExpiresInSeconds= 3599,
                RefreshToken= "1//04xeb8_W7Dr52CgYIARAAGAQSNwF-L9IryEHbbhps-TTxH6iIr6UT3iArN_1mk6XmkKhrZjoqmKvP3utkdUO-vmYYP2-za_Pw2js",
            };

            var applicationName = "GDrive";// Use the name of the project in Google Cloud
           // var applicationName = "CNJ";// Use the name of the project in Google Cloud
            var username = "shyamal.ikart@gmail.com"; // Use your email
            //var username = "shaileshparmar13101997@gmail.com"; // Use your email

            var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "1003902974858-5524oc4vferstdlr3hh3p4fv3sof9g1a.apps.googleusercontent.com",
                    //ClientId = "219931079707-223o6dpov3lff8504glrr3d3g7c7ela3.apps.googleusercontent.com",
                    ClientSecret = "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",
                    // ClientSecret = "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"
                },
                Scopes = new[] { Scope.Drive },
                DataStore = new FileDataStore(applicationName),
            });

            var credential = new UserCredential(apiCodeFlow, username, tokenResponse);
            var baseUri = "https://localhost:7113";
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
                BaseUri= baseUri
            });
            return service;
        }

        public Google.Apis.Drive.v3.DriveService GetService23()
        {
            //get Credentials from client_secret.json file     
            UserCredential credential;
            //var CSPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/");
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
                ApplicationName = "GoogleDriveRestAPI-v3",
            });
            return service;
        }

    }
}
