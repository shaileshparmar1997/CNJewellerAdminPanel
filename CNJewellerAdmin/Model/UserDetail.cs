using System;
using System.Collections.Generic;

namespace CNJewellerAdmin.Model
{
    public partial class UserDetail
    {
        public int Id { get; set; }
        public int OfficeId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string MobileNo { get; set; } = null!;
        public string? EmailId { get; set; }
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime? JoiningDate { get; set; }
        public int RoleId { get; set; }
        public int UserType { get; set; }
        public int? Gender { get; set; }
        public string Password { get; set; } = null!;
        public string? ProfilePic { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int RowStatus { get; set; }
    }
}
