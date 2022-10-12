using Microsoft.EntityFrameworkCore.Metadata;

namespace CNJewellerAdmin.Models
{
    public class GoogleDriveFileNew
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? Size { get; set; }
        public long? Version { get; set; }
        public DateTime? CreatedTime { get; set; }
        public IList<string> Parents { get; set; }
        public string MimeType { get; set; }
        public List<User> owners { get; set; }
        public string ThumbnailLink { get; set; }
        public string FileExtensions { get; set; }
    }
    public class User
    {
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }

    }
}
