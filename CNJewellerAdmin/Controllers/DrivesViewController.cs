using CNJewellerAdmin.Helper;
using CNJewellerAdmin.Helper.GDrive;
using CNJewellerAdmin.Model;
using CNJewellerAdmin.Models;
using Microsoft.AspNetCore.Mvc;

namespace CNJewellerAdmin.Controllers
{
    public class DrivesViewController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly GoogleDriveFilesRepository _GDriveHelper;

        public DrivesViewController(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            this._hostEnvironment = hostEnvironment;
            this._configuration = configuration;
            _GDriveHelper = new GoogleDriveFilesRepository(hostEnvironment);

        }

        public IActionResult Index(Guid sharedId)
        {
            DriveFilesDTO response = new DriveFilesDTO();
            try
            {
                using (var db = new CNJContext())
                {
                    var sharedData = db.DriveFiles.FirstOrDefault(x => x.SharedGuid == sharedId && x.IsActive == true);
                    if (sharedData != null)
                    {
                        if (sharedData.ExpiryTime >= DateTime.Now)
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
                            var ms = ts.Milliseconds;

                            response.ExpiryLimit = "Days:" + d + ", " + h + ":" + m + ":" + ms;

                            var sharedList = db.ShareData.Where(x => x.SharedGuid == sharedId).ToList();
                            response.sharedItems = new List<SharedItem>();
                            foreach (var item in sharedList)
                            {
                                SharedItem list = new SharedItem();
                                list.SharedGuid = item.SharedGuid;
                                list.SharedData = item.SharedData;
                                list.Name = item.Name;
                                list.MIMEType = item.Mimetype;
                                list.ThumbnailLink = !string.IsNullOrEmpty(item.ThumbnailLink) ? item.ThumbnailLink : "";
                                list.SharedGuid = item.SharedGuid;
                                list.LocalFilePath = !string.IsNullOrEmpty(item.LocalFilePath) ? Path.Combine(_hostEnvironment.WebRootPath, item.LocalFilePath) : "";
                                response.sharedItems.Add(list);
                            }
                        }
                        else
                        {
                            response.IsLinkExpire = true;
                        }
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
            return View(response);
        }

        public IActionResult GetContainsInFolder(string folderId)
        {
            List<GoogleDriveFileNew> result = new List<GoogleDriveFileNew>();
            result = _GDriveHelper.GetContainsInFolder(folderId);
            return View("Index", result);
        }


    }
}
