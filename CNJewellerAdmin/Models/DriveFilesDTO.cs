namespace CNJewellerAdmin.Models
{
    public class DriveFilesDTO
    {
        public string UserName { get; set; }
        public int Mobile { get; set; }
        public DateTime ExpiryTime { get; set; }
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
