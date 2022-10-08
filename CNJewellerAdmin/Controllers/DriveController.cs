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
            var driveFiles = GetFiles("temp");
            return View(driveFiles);
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
                AccessToken = "ya29.a0Aa4xrXMUgLEDcrRAeB6FAKDPh-fptun8Fqv9IxWkD1g5t5zDtAqOJA4CngZW98-aiga5ROVVdOALXf7OPXHQTX4obrdyNNvyuCFM2sK7frNYcvxZVJpIHLYUJLnnnguMRSKl7DEHX3BH3NiQLrV8QRcIaUF_aCgYKATASARESFQEjDvL9o86Tml6LxhQH1ywz-l3HIQ0163",
                RefreshToken = "1//04wau0eGEmhsVCgYIARAAGAQSNwF-L9IrIodwPT1-JtKAGMDU2KlNcuYjiv2mUAjybr78kGptxsgunn3T_5dsFC1TiGZGjrBa6C4",
            };

            var applicationName = "CNJ";// Use the name of the project in Google Cloud
            var username = "shaileshparmar13101997@gmail.com"; // Use your email

            var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "219931079707-223o6dpov3lff8504glrr3d3g7c7ela3.apps.googleusercontent.com",
                    ClientSecret = "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"
                },
                Scopes = new[] { Scope.Drive },
                DataStore = new FileDataStore(applicationName)
            });

            var credential = new UserCredential(apiCodeFlow, username, tokenResponse);

            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
            return service;
        }

    }
}
