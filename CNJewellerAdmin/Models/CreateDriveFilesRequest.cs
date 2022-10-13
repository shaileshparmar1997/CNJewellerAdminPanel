namespace CNJewellerAdmin.Models
{
    public class CreateDriveFilesRequest
    {
        public long Id { get; set; }
        public Guid SharedGuid { get; set; }
        public string UserName { get; set; } = null!;
        public int Mobile { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public List<CreateSharedDataRequest> sharedItems { get; set; }
    }
    public class CreateSharedDataRequest
    {
        public string SharedData { get; set; }
        public string Name { get; set; } = null!;
        public string? ThumbnailLink { get; set; }
        public string Mimetype { get; set; } = null!;
    }
}
