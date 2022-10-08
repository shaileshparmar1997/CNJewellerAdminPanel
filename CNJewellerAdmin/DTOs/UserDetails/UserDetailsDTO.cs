using CNJewellerAdmin.Helper;

namespace CNJewellerAdmin.DTOs.UserDetails
{
    public class UserDetailsDTO
    {
        public int Id { get; set; }
        public int OfficeId { get; set; }
        public string OfficeName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string MobileNo { get; set; } = null!;
        public string? EmailId { get; set; }
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string JoiningDate { get; set; }
        public int RoleId { get; set; }
        public string  RoleName { get; set; }
        public int UserType { get; set; }
        public int? Gender { get; set; }
        public string GenderName { get; set; } = null!;
        public string? ProfilePic { get; set; }
        public int RowStatus { get; set; }
        public string RowStatusName {
            get { return ((RowStatus)RowStatus).ToString(); }
        }
    }
}
