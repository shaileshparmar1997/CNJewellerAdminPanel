using CNJewellerAdmin.DTOs.Account.Token;
using CNJewellerAdmin.Helper.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace CNJewellerAdmin.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult LoginView()
        {
            UserValidateRequest model = new UserValidateRequest();

            if (Request.Cookies["userName"] != null)
            {
                model.EmailAddress = Request.Cookies["userName"];
            }

            if (Request.Cookies["password"] != null)
            {
                model.Password = Request.Cookies["password"];
            }
            return View();
        }

        public async Task<IActionResult> ValidateUser(UserValidateRequest request)
        {
            TokenResponse result = new TokenResponse();
            try
            {
                var route = "Token";
                var response = await DataHelper<TokenResponse>.AuthenticateUser(route, request);
                if (response != null && response.Result.Flag && response.Data != null)
                {
                    result = response.Data;
                    if (request.IsRemember && response.Data != null)
                    {
                        var option = new CookieOptions();
                        option.Expires = DateTime.Now.AddDays(15);
                        option.IsEssential = true;
                        Response.Cookies.Append("userName", request.EmailAddress, option);

                        var option1 = new CookieOptions();
                        option1.Expires = DateTime.Now.AddDays(15);
                        option1.IsEssential = true;
                        Response.Cookies.Append("password", request.Password, option1);
                    }
                    HttpContext.Session.SetString("access_Token", response.Data.Access_Token);
                    HttpContext.Session.SetString("firstName", response.Data.FirstName);
                    HttpContext.Session.SetString("lastName", response.Data.LastName);
                    HttpContext.Session.SetString("legalUserEmail", response.Data.Email);
                    HttpContext.Session.SetInt32("legalUserRoleId", response.Data.RoleId);
                    HttpContext.Session.SetInt32("legalUserId", response.Data.Id);
                    HttpContext.Session.SetInt32("legalId", response.Data.LegelId);
                    HttpContext.Session.SetString("legalUserRole", response.Data.UserType);
                    var AuthToken = HttpContext.Session.GetString("access_Token");
                    //if (response.Data.RoleId > 0)
                    //{
                    //    var authToken = HttpContext.Session.GetString("access_Token");
                    //    var roleRoute = "Role/CurrentUserRole?id=" + response.Data.RoleId;
                    //    var roleResponse = await DataHelper<GetRoleDetailsResponse>.Execute(roleRoute, OperationTypes.GET, authToken);
                    //    if (roleResponse != null && roleResponse.Result.Flag && roleResponse.Data != null)
                    //    {
                    //        var roleResult = roleResponse.Data.ModulesNames;
                    //        HttpContext.Session.SetObjectAsJson("UserRoleData", roleResponse.Data);
                    //        ViewData["CurUserRoleModule"] = HttpContext.Session.GetObjectFromJson<GetRoleDetailsResponse>("UserRoleData");
                    //        //ViewData["CurUserRoleModule"] = roleResponse.Data.RoleData;
                    //        //ViewBag.CurrentUserRoleData = roleResponse.Data.RoleData;
                    //        //ViewBag.RoleModuleData = roleResponse.Data.ModulesNames;
                    //    }
                    //}
                    string userRolePermission = "";

                    if (response.Data.RoleName.ToUpper() == "ADMIN")
                    {
                        userRolePermission = "Write,Delete,Read";
                    }
                    else
                    {
                        userRolePermission = "Write,Delete,Read";
                    }
                }
                else
                {

                }
            }
            catch (Exception)
            {
            }

            return Json(result);
        }
    }
}
