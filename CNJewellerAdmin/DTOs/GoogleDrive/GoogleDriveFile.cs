using Google.Apis.Util;
using Newtonsoft.Json;

namespace CNJewellerAdmin.DTOs.GoogleDrive
{
    public class GoogleDriveFile
    {
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public long? Size { get; set; }

        public long? Version { get; set; }

        [JsonProperty("createdTime")]
        public virtual string CreatedTimeRaw { get; set; }

        [JsonIgnore]
        public virtual DateTime? CreatedTime {
            get {
                return Utilities.GetDateTimeFromString(CreatedTimeRaw);
            }
            set {
                CreatedTimeRaw = Utilities.GetStringFromDateTime(value);
            }
        }

        [JsonProperty("originalFilename")]
        public virtual string OriginalFilename { get; set; }

        [JsonProperty("parents")]
        public IList<string> Parents { get; set; }
        [JsonProperty("mimeType")]
        public virtual string MimeType { get; set; }

        [JsonProperty("owners")]
        public virtual IList<User> Owners { get; set; }


        [JsonProperty("permissionIds")]
        public virtual IList<string> PermissionIds { get; set; }

        [JsonProperty("thumbnailLink")]
        public virtual string ThumbnailLink { get; set; }

        [JsonProperty("thumbnailVersion")]
        public virtual long? ThumbnailVersion { get; set; }
    }

    public class User
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("emailAddress")]
        public virtual string EmailAddress { get; set; }
        [JsonProperty("kind")]
        public virtual string Kind { get; set; }
        [JsonProperty("me")]
        public virtual bool? Me { get; set; }
        [JsonProperty("permissionId")]
        public virtual string PermissionId { get; set; }
        [JsonProperty("photoLink")]
        public virtual string PhotoLink { get; set; }
        public virtual string ETag { get; set; }
    }
}
