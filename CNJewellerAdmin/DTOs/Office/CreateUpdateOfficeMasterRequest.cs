namespace CNJewellerAdmin.DTOs.Office
{
    public class CreateUpdateOfficeMasterRequest
    {
        public int Id { get; set; }
        public string OfficeName { get; set; } = null!;
        public string PhoneNo { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string BranchCode { get; set; } = null!;
        public int? UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public string UpdatedDate { get; set; }
    }
}
