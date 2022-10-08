using CNJewellerAdmin.DTOs.Base;

namespace CNJewellerAdmin.DTOs.Account.Token
{
    public class TokenResponse : BaseResponse
    {
        public int LegelId { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? MobileNo { get; set; }
        public string UserType { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public int AccessFailedCount { get; set; }
        public string LockOutEndDate { get; set; }
        public bool IsProfileUpdated { get; set; }
        public bool AccountLocked { get; set; }
        public bool IsPasswordExpired { get; set; }
        public string LastLoginDate { get; set; }
        public string? ProfilePic { get; set; }
        public int PasswordExpireIn { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string? Address { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public string CityName { get; set; }
        public string PostCode { get; set; }
        public string RefreshToken { get; set; }
        public string Access_Token { get; set; }
        public DateTime Expires_In { get; set; }
        public string ResetPasswordLink { get; set; }
        public string DeactivateDate { get; set; }
        public string Refresh_Token { get; set; }
    }

}
