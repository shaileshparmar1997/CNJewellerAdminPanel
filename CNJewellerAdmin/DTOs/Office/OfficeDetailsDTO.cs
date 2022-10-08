using CNJewellerAdmin.Helper;

namespace CNJewellerAdmin.DTOs.Office
{
    public class OfficeDetailsDTO
    {
        public int Id { get; set; }
        public string OfficeName { get; set; } = null!;
        public string PhoneNo { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string BranchCode { get; set; } = null!;
        public int RowStatus { get; set; }
        public string RowStatusName {
            get { return ((RowStatus)RowStatus).ToString(); }
        }
    }
}
