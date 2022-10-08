using CNJewellerAdmin.DTOs.UserDetails;
using CNJewellerAdmin.Helper;
using CNJewellerAdmin.Helper.DateUtil;
using CNJewellerAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNJewellerAdmin.Controllers
{
    public class BaseController : Controller
    {
        public string AuthToken { get { return HttpContext.Session.GetString("access_Token"); } }
        public DateTime CurrentDate { get { return DateUtil.GetCurrentDate(); } }
        public int CurrentUserId { get { return HttpContext.Session.GetInt32("CurrentUserId") ?? 0; } }
        public BaseController()
        {
            //AuthToken= HttpContext.Session.GetString("access_Token");
        }

        public async Task<UserDetailsDTO> GetUserDetails(int id)
        {
            UserDetailsDTO response = new UserDetailsDTO();
            try
            {
                using (var db = new CNJewellerDBContext())
                {
                    var userDetails = await db.UserDetails.FirstOrDefaultAsync(x => x.Id == id && x.RowStatus == (int)RowStatus.Active);
                    if (userDetails != null)
                    {
                        response.Id = userDetails.Id;
                        response.FirstName = userDetails.FirstName;
                        response.MiddleName = userDetails.MiddleName;
                        response.LastName = userDetails.LastName;
                    }
                }
            }
            catch (Exception ex)
            {
                response = null;
            }
            return response;
        }
    }
}
