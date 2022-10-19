using CNJewellerAdmin.DTOs.Base;

namespace CNJewellerAdmin.Models
{
    public class DriveFilesDTO : BaseResponse
    {
        public string FolderId { get; set; }
        public Guid? SharedGuid { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string ExpiryTime { get; set; }
        public string CurrentDateTime { get; set; }
        public string ExpiryLimit { get; set; }
        public string CountDays { get; set; }
        public string Link { get; set; }
        public bool IsLinkExpire { get; set; }
        public List<SharedItem> sharedItems { get; set; }
      
    }
    public class SharedItem
    {
        public string GDriveLink { get; set; }
        public string LocalFolderLink { get; set; }
        public Guid? SharedGuid { get; set; }
        public string SharedData { get; set; }
        public string Name { get; set; }
        public string ThumbnailLink { get; set; }
        public string MIMEType { get; set; }
        public string? LocalFilePath { get; set; }
    }
}
