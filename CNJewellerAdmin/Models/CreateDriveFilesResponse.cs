using CNJewellerAdmin.DTOs.Base;

namespace CNJewellerAdmin.Models
{
    public class CreateDriveFilesResponse : BaseResponse
    {
        public Guid? SharedData { get; set; }
    }
}
