using System;
using System.Collections.Generic;

namespace CNJewellerAdmin.Model
{
    public partial class DriveFile
    {
        public long Id { get; set; }
        public Guid? SharedGuid { get; set; }
        public string UserName { get; set; } = null!;
        public string Mobile { get; set; } = null!;
        public DateTime ExpiryTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
