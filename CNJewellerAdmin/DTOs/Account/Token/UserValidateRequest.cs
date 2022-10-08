namespace CNJewellerAdmin.DTOs.Account.Token
{
    public class UserValidateRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool IsRemember { get; set; }
    }
}
