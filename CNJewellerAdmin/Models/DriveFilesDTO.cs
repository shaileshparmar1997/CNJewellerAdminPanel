using CNJewellerAdmin.DTOs.Base;

namespace CNJewellerAdmin.Models
{
    public class DriveFilesDTO : BaseResponse
    {
        public Guid? SharedGuid { get; set; }
        public string UserName { get; set; }
        public int Mobile { get; set; }
        public string ExpiryTime { get; set; }
        public string CurrentDateTime { get; set; }
        public string CountDays { get; set; }
        public List<SharedItem> sharedItems { get; set; }
      
    }
    public class SharedItem
    {
        public string SharedData { get; set; }
        public string Name { get; set; }
        public string ThumbnailLink { get; set; }
        public string MIMEType { get; set; }
    }
}
