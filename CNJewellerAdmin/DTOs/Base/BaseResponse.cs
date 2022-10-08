using CNJewellerAdmin.Helper;

namespace CNJewellerAdmin.DTOs.Base
{
    public class BaseResponse
    {
        public int Id { get; set; }
        public AcknowledgeType Acknowledge { get; set; }
        public string Message { get; set; }
    }
}
