using CNJewellerAdmin.DTOs.Account.Token;
using CNJewellerAdmin.DTOs.Base;
using CNJewellerAdmin.Helper;
using CNJewellerAdmin.Helper.DataAccess;
using CNJewellerAdmin.Model;
using CNJewellerAdmin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace CNJewellerAdmin.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult LoginView()
        {
            UserValidateRequest model = new UserValidateRequest();

            if (Request.Cookies["userName"] != null)
            {
                model.MobileNo = Request.Cookies["userName"];
            }

            if (Request.Cookies["password"] != null)
            {
                model.Password = Request.Cookies["password"];
            }
            return View();
        }

        public async Task<IActionResult> ValidateUser(UserValidateRequest request)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                using (var db = new CNJContext())
                {
                    var user = db.UserDetails.FirstOrDefault(x => x.MobileNo.Trim() == request.MobileNo.Trim() && x.RowStatus == (int)RowStatus.Active);
                    if (user != null)
                    {
                        var passwordHasher = new PasswordHasher<string>();
                        var passwordValidateResult = passwordHasher.VerifyHashedPassword(user.MobileNo, user.Password, request.Password);
                        if (passwordValidateResult == PasswordVerificationResult.Success)
                        {
                            if (request.IsRemember)
                            {
                                var option = new CookieOptions();
                                option.Expires = DateTime.Now.AddDays(15);
                                option.IsEssential = true;
                                Response.Cookies.Append("userName", request.MobileNo, option);

                                var option1 = new CookieOptions();
                                option1.Expires = DateTime.Now.AddDays(15);
                                option1.IsEssential = true;
                                Response.Cookies.Append("password", request.Password, option1);
                            }
                            HttpContext.Session.SetString("firstName", user.FirstName);
                            HttpContext.Session.SetString("middleName", user.MiddleName);
                            HttpContext.Session.SetString("lastName", user.LastName);
                            HttpContext.Session.SetString("currentMobile", user.MobileNo);
                            HttpContext.Session.SetString("currentEmailId", user.EmailId??"");
                            HttpContext.Session.SetInt32("currentRoleId", user.RoleId);
                            HttpContext.Session.SetString("lastName", user.LastName);
                            HttpContext.Session.SetString("lastName", user.LastName);
                            HttpContext.Session.SetString("lastName", user.LastName);
                            HttpContext.Session.SetString("lastName", user.LastName);
                            response.Acknowledge = AcknowledgeType.Success;
                            response.Message = "Success";
                        }
                        else
                        {
                            response.Acknowledge = AcknowledgeType.Failure;
                            response.Message = "Mobile no or password does not match, Please verify";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Acknowledge = AcknowledgeType.Failure;
                response.Message = "Somethinf wrong , please try again after some time";
            }
            return Json(response);
        }
    }
}
