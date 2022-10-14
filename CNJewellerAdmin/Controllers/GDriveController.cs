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

namespace CNJewellerAdmin.Controllers
{
    public class GDriveController : Controller
    {
        private readonly GoogleDriveFilesRepository _GDriveHelper;
        public string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };


        private readonly IWebHostEnvironment _hostEnvironment;
        public GDriveController(IWebHostEnvironment hostEnvironment)
        {
            this._hostEnvironment = hostEnvironment;
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

        [HttpGet]
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
                        foreach (var item in request.sharedItems)
                        {
                            var subData = new ShareDatum
                            {
                                SharedData = item.SharedData,
                                Name = item.Name,
                                Mimetype = item.Mimetype,
                                ThumbnailLink = item.ThumbnailLink,
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
        public ActionResult GetSharedData(Guid SharedId)
        {
            CreateDriveFilesRequest response = new CreateDriveFilesRequest();
            try
            {
                using (var db = new CNJContext())
                {
                    var sharedData = db.DriveFiles.FirstOrDefault(x => x.SharedGuid == SharedId && x.IsActive == true);

                    response.SharedGuid = sharedData.SharedGuid;
                    response.UserName = sharedData.UserName;
                    response.Mobile = sharedData.Mobile;
                    response.ExpiryTime = sharedData.ExpiryTime.ToString("d-m-yy HH:mm");
                }
            }
            catch (Exception ex)
            {
            }
            return Json(response);
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
