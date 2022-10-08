using CNJewellerAdmin.DTOs.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CNJewellerAdmin.DTOs.UserDetails
{
    public class CreateUpdateUserDetailRequest
    {
        public int Id { get; set; }

        public int OfficeId { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string FirstName { get; set; } = null!;
      
        [StringLength(15, MinimumLength = 3)]
        public string MiddleName { get; set; } = null!;
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string LastName { get; set; } = null!;
        [Required]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Mobile no not valid")]
        public string MobileNo { get; set; } = null!;
        [EmailAddress]
        public string? EmailId { get; set; }
        public string City { get; set; } = null!;
        public string Pincode { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string JoiningDate { get; set; }
        public int RoleId { get; set; }
        public int UserType { get; set; }
        public int? Gender { get; set; }
      
        [Required]
        [StringLength(10, MinimumLength = 6)]
        public string Password { get; set; } = null!;
        [Required]
        [NotMapped] 
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string? ProfilePic { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public string UpdatedDate { get; set; }
        public List<DropDownBase> Genders { get; set; }
        public List<DropDownBase> Roles { get; set; }
        public List<DropDownBase> Offices { get; set; }
    }
}
