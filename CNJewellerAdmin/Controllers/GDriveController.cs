using CNJewellerAdmin.DTOs.Base;
using CNJewellerAdmin.Helper.DateUtil;
using CNJewellerAdmin.Helper;
using CNJewellerAdmin.Helper.GDrive;
using CNJewellerAdmin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using CNJewellerAdmin.Model;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.IO;
using System.Security.Policy;

namespace CNJewellerAdmin.Controllers
{
    public class GDriveController : Controller
    {
        private readonly GoogleDriveFilesRepository _GDriveHelper;
        public string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };
        public string MainFilePath = string.Empty;
        public string LocalFolderPath = string.Empty;

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;
        public GDriveController(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            this._hostEnvironment = hostEnvironment;
            this._configuration = configuration;
            _GDriveHelper = new GoogleDriveFilesRepository(hostEnvironment);
        }

        [HttpGet]
        public ActionResult GetGoogleDriveFiles()
        {
            return View(_GDriveHelper.GetDriveFiles());
        }

        [HttpGet]
        public ActionResult GetGoogleDriveFiles1()
        {
            return View(_GDriveHelper.GetDriveFiles());
        }


        [HttpGet]
        public ActionResult GetGoogleDriveFiles2()
        {
            return View(_GDriveHelper.GetDriveFiles());
        }

        [HttpPost]
        public ActionResult DeleteFile(GoogleDriveFileNew file)
        {
            _GDriveHelper.DeleteFile(file);
            return RedirectToAction("GetGoogleDriveFiles");
        }

        [HttpPost]
        public ActionResult UploadFile(IFormFile file)
        {
            _GDriveHelper.FileUpload(file);
            return RedirectToAction("GetGoogleDriveFiles");
        }

        public void DownloadFile(string id)
        {
            string FilePath = _GDriveHelper.DownloadGoogleFile(id);

            //Response.ContentType = "application/zip";
            //Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
            //Response.WriteFile(System.Web.HttpContext.Current.Server.MapPath("~/GoogleDriveFiles/" + Path.GetFileName(FilePath)));
            //Response.End();
            //Response.Flush();
        }

        [HttpPost]
        public ActionResult OpenDrive(DriveFilesDTO request)
        {
            return PartialView("~/Views/GDrive/_filesData.cshtml", request);
        }


        private async Task<List<DriveFilesDTO>> CreateLocalGDrives(CreateSharedDataRequest request, string mainFilePath = "")
        {
            List<GoogleDriveFileNew> driveFiles = new List<GoogleDriveFileNew>();
            driveFiles = _GDriveHelper.GetContainsInFolder(request.SharedData);
            string SubFolderPath = "";
            if (driveFiles.Count() > 0)
            {
                SubFolderPath = mainFilePath;
            }
            foreach (var item in driveFiles)
            {
                if (item.MimeType == "application/vnd.google-apps.folder")
                {
                    // MainFilePath
                    string subFolderPath = SubFolderPath + "/" + item.Name;
                    if (!Directory.Exists(subFolderPath))
                    {
                        Directory.CreateDirectory(subFolderPath);
                    }
                    //SubFolderPath = subFolderPath;
                    CreateSharedDataRequest newRequest = new CreateSharedDataRequest();
                    newRequest.SharedGuid = Guid.NewGuid();
                    newRequest.SharedData = item.Id;
                    newRequest.Name = item.Name;
                    newRequest.ThumbnailLink = item.ThumbnailLink;
                    newRequest.Mimetype = item.MimeType;
                    var resultData = await CreateLocalGDrives(newRequest, subFolderPath);
                }
                else
                {
                    var imgFilePath = SubFolderPath + "/" + item.Name;
                    using (var client = new HttpClient())
                    {
                        using (var responseUrl = await client.GetAsync(item.ThumbnailLink))
                        {
                            byte[] imageBytes = await responseUrl.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                            System.IO.File.WriteAllBytes(imgFilePath, imageBytes);
                        }
                    }
                }
            }
            return null;
        }


        [HttpPost]
        public async Task<JsonResult> SaveDrive(CreateDriveFilesRequest request)
        {
            CreateDriveFilesResponse response = new CreateDriveFilesResponse();
            if (request != null)
            {
                try
                {
                    using (var db = new CNJContext())
                    {
                        DriveFile drivesDetail = new DriveFile();
                        drivesDetail.SharedGuid = Guid.NewGuid();
                        drivesDetail.UserName = request.UserName;
                        drivesDetail.Mobile = request.Mobile;
                        drivesDetail.ExpiryTime = DateUtil.GetStringTODate(request.ExpiryTime);
                        drivesDetail.CreatedBy = 1;
                        drivesDetail.CreatedDate = DateTime.Now;
                        drivesDetail.IsActive = true;
                        await db.DriveFiles.AddAsync(drivesDetail);

                        List<DriveFilesDTO> result = new List<DriveFilesDTO>();
                        foreach (var item in request.sharedItems)
                        {
                            string localFilePath = "";
                            var datetime = DateTime.Now.ToString("ddMMyyyyHHmm");
                            var mobDrivePath = request.Mobile + "_" + datetime;
                            string createCustFolder = _configuration.GetValue<string>("FileLocation:DriveFolder") + mobDrivePath;
                            var physicalCustPath = Path.Combine(_hostEnvironment.WebRootPath, createCustFolder);
                            localFilePath = createCustFolder;
                            if (!Directory.Exists(physicalCustPath))
                            {
                                Directory.CreateDirectory(physicalCustPath);
                            }
                            MainFilePath = physicalCustPath;
                            if (item.Mimetype == "application/vnd.google-apps.folder")
                            {
                                string tempFolderPath = createCustFolder + "/" + item.Name;
                                var physicalPath = Path.Combine(_hostEnvironment.WebRootPath, tempFolderPath);
                                if (!Directory.Exists(physicalPath))
                                {
                                    Directory.CreateDirectory(physicalPath);
                                }
                                MainFilePath = physicalPath;
                                var abcd = await CreateLocalGDrives(item, MainFilePath);
                            }
                            else
                            {
                                var imgFilePath  = MainFilePath + "/" + item.Name;
                                localFilePath = localFilePath + "/" + item.Name;
                                using (var client = new HttpClient())
                                {
                                    using (var responseUrl = await client.GetAsync(item.ThumbnailLink))
                                    {
                                        byte[] imageBytes = await responseUrl.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                                        System.IO.File.WriteAllBytes(imgFilePath, imageBytes);
                                    }
                                }
                            }

                            var subData = new ShareDatum
                            {
                                SharedGuid = drivesDetail.SharedGuid,
                                SharedData = item.SharedData,
                                Name = item.Name,
                                Mimetype = item.Mimetype,
                                ThumbnailLink = !string.IsNullOrEmpty(item.ThumbnailLink) ? item.ThumbnailLink : "",
                                LocalFilePath = localFilePath,
                            };
                            await db.ShareData.AddAsync(subData);
                        }
                        await db.SaveChangesAsync();
                        response.SharedData = drivesDetail.SharedGuid;
                        response.Acknowledge = Helper.AcknowledgeType.Success;
                        response.Message = "Successfully create new Drive Files";
                    }
                }
                catch (Exception ex)
                {
                    response.Acknowledge = Helper.AcknowledgeType.Failure;
                    response.Message = ex.Message;
                }
            }
            else
            {
                response.Acknowledge = Helper.AcknowledgeType.Failure;
                response.Message = "Request is not valid";
            }
            return Json(response);
        }



        [HttpPost]
        public ActionResult GetSharedData(Guid sharedId)
        {
            DriveFilesDTO response = new DriveFilesDTO();
            try
            {
                using (var db = new CNJContext())
                {
                    var sharedData = db.DriveFiles.FirstOrDefault(x => x.SharedGuid == sharedId && x.IsActive == true);
                    if (sharedData != null)
                    {
                        response.SharedGuid = sharedData.SharedGuid;
                        response.UserName = sharedData.UserName;
                        response.Mobile = sharedData.Mobile;
                        response.ExpiryTime = sharedData.ExpiryTime.ToString("dd-MM-yyyy HH:mm");
                        response.CurrentDateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                        TimeSpan ts = sharedData.ExpiryTime - DateTime.Now;
                        var d = ts.Days;
                        var h = ts.Hours;
                        var m = ts.Minutes;

                        response.ExpiryLimit = "Days:" + d + ", " + h + ":" + m;

                        var keyLink = _configuration["HostUrl"];
                        response.Link = keyLink + "DrivesView/Index?sharedId=" + response.SharedGuid;
                        //var sharedList = db.ShareData.Where(x => x.SharedGuid == sharedId).ToList();
                        //response.sharedItems = new List<SharedItem>();
                        //foreach (var item in sharedList)
                        //{
                        //    SharedItem list = new SharedItem();
                        //    list.SharedGuid = item.SharedGuid;
                        //    list.SharedData = item.SharedData;
                        //    list.Name = item.Name;
                        //    list.MIMEType = item.Mimetype;
                        //    list.ThumbnailLink = !string.IsNullOrEmpty(item.ThumbnailLink) ? item.ThumbnailLink : "";
                        //    list.SharedGuid = item.SharedGuid;
                        //    response.sharedItems.Add(list);
                        //}
                    }
                    else
                    {
                        response.Acknowledge = AcknowledgeType.Failure;
                        response.Message = "No record found";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return PartialView("~/Views/GDrive/_getSharedList.cshtml", response);
        }

        public IActionResult GetContainsInFolder(string folderId)
        {
            List<GoogleDriveFileNew> result = new List<GoogleDriveFileNew>();
            result = _GDriveHelper.GetContainsInFolder(folderId);
            return View("GetContainsInFolder", result);
        }

        [HttpPost]
        public ActionResult CreateFolder(String FolderName)
        {
            _GDriveHelper.CreateFolder(FolderName);
            return RedirectToAction("GetGoogleDriveFiles1");
        }


        [HttpPost]
        public ActionResult FileUploadInFolder(GoogleDriveFileNew FolderId, IFormFile file)
        {
            // _GDriveHelper.FileUploadInFolder(FolderId.Id, file);
            return RedirectToAction("GetGoogleDriveFiles1");
        }


        [HttpGet]
        public IActionResult FolderLists()
        {
            List<GoogleDriveFileNew> AllFolders = _GDriveHelper.GetDriveFolders();
            List<DDLOptions> obj = new List<DDLOptions>();

            foreach (GoogleDriveFileNew EachFolder in AllFolders)
            {
                obj.Add(new DDLOptions { Id = EachFolder.Id, Name = EachFolder.Name, Size = EachFolder.Size });
            }
            return View(obj);
        }

        public string MoveFiles(String fileId, String folderId)
        {
            string Result = _GDriveHelper.MoveFiles(fileId, folderId);
            return Result;
        }

        public string CopyFiles(String fileId, String folderId)
        {
            string Result = _GDriveHelper.CopyFiles(fileId, folderId);
            return Result;
        }

        //public JsonResult Render_GetGoogleDriveFilesView()
        //{
        //    Dictionary<string, object> jsonResult = new Dictionary<string, object>();
        //    var result = _GDriveHelper.GetDriveFiles();
        //    jsonResult.Add("Html", RenderRazorViewToString("~/Views/Home/GetGoogleDriveFiles1.cshtml", result));
        //    return Json(jsonResult);
        //}

        //public string RenderRazorViewToString(string viewName, object model)
        //{
        //    if (model != null)
        //    {
        //        ViewData.Model = model;

        //    }
        //    using (var sw = new StringWriter())
        //    {
        //        var viewResult = Microsoft.AspNetCore.Mvc.ViewEngines.FindPartialView(ControllerContext, viewName);
        //        var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}
    }
}
