using System;
using System.Collections.Generic;

namespace CNJewellerAdmin.Models
{
    public partial class OfficeMaster
    {
        public OfficeMaster()
        {
            UserDetails = new HashSet<UserDetail>();
        }

        public int Id { get; set; }
        public string OfficeName { get; set; } = null!;
        public string PhoneNo { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string BranchCode { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int RowStatus { get; set; }

        public virtual ICollection<UserDetail> UserDetails { get; set; }
    }
}
