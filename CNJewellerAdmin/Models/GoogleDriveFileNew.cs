﻿namespace CNJewellerAdmin.Models
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
    }
}
