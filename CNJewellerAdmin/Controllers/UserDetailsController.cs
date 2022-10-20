using CNJewellerAdmin.DTOs.Base;
using CNJewellerAdmin.DTOs.UserDetails;
using CNJewellerAdmin.Helper;
using CNJewellerAdmin.Helper.DataAccess;
using CNJewellerAdmin.Helper.DateUtil;
using CNJewellerAdmin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using NuGet.Protocol.Plugins;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace CNJewellerAdmin.Controllers
{
    public class UserDetailsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetUserList(int officeId)
        {
            List<UserDetailsDTO> result = new List<UserDetailsDTO>();
            try
            {
                using (var db = new CNJewellerDBContext())
                {
                    var userDetails = db.UserDetails.Where(x => x.RowStatus == (int)RowStatus.Active);
                    var officeDetails = db.OfficeMasters.Where(x => x.RowStatus == (int)RowStatus.Active);
                    var userData = (from ud in userDetails
                                    join om in officeDetails on ud.OfficeId equals om.Id
                                    select new UserDetailsDTO
                                    {
                                        Id = ud.Id,
                                        OfficeId = ud.OfficeId,
                                        OfficeName = om.OfficeName,
                                        FirstName = ud.FirstName,
                                        MiddleName = ud.MiddleName,
                                        LastName = ud.LastName,
                                        MobileNo = ud.MobileNo,
                                        EmailId = ud.EmailId,
                                        City = ud.City,
                                        Pincode = ud.Pincode,
                                        Address = ud.Address,
                                        JoiningDate = DateUtil.GetDateString(ud.JoiningDate),
                                        RoleId = ud.RoleId,
                                        UserType = ud.UserType,
                                        Gender = ud.Gender,
                                        GenderName = ((Genders)ud.Gender).ToString(),
                                        ProfilePic = ud.ProfilePic,
                                        RowStatus = ud.RowStatus
                                    });

                    //if (request.OfficeId > 0)
                    //{
                    //    userData = userData.Where(x => x.OfficeId == request.OfficeId);
                    //}
                    result = userData.ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return Json(result);
        }

        public async Task<IActionResult> OpenUserDetailsForm(int id)
        {
            BaseResponse response = new BaseResponse();
            CreateUpdateUserDetailRequest request = new CreateUpdateUserDetailRequest();
            request.Genders = new List<DropDownBase>();
            request.Roles = new List<DropDownBase>();
            request.Offices = new List<DropDownBase>();
            try
            {
                var genderList = ((Genders[])Enum.GetValues(typeof(Genders))).Select(c => new DropDownBase() { Id = (int)c, Name = c.ToString() }).ToList();
                request.Genders = genderList;
                using (var db = new CNJewellerDBContext())
                {
                    request.Offices = await db.OfficeMasters.Where(x => x.RowStatus == (int)RowStatus.Active).Select(x => new DropDownBase
                    {
                        Id = x.Id,
                        Name = x.OfficeName
                    }).ToListAsync();

                    if (id > 0)
                    {
                        var getUserDetails = await db.UserDetails.FirstOrDefaultAsync(x => x.Id == id && x.RowStatus == (int)RowStatus.Active);
                        if (getUserDetails != null)
                        {
                            request.Id = getUserDetails.Id;
                            request.OfficeId = getUserDetails.OfficeId;
                            request.FirstName = getUserDetails.FirstName;
                            request.MiddleName = getUserDetails.MiddleName;
                            request.LastName = getUserDetails.LastName;
                            request.MobileNo = getUserDetails.MobileNo;
                            request.EmailId = getUserDetails.EmailId;
                            request.City = getUserDetails.City;
                            request.Pincode = getUserDetails.Pincode;
                            request.Address = getUserDetails.Address;
                            request.JoiningDate = DateUtil.GetDateString(getUserDetails.JoiningDate);
                            request.RoleId = getUserDetails.RoleId;
                            request.UserType = getUserDetails.UserType;
                            request.Gender = getUserDetails.Gender;
                            request.Password = getUserDetails.Password;
                            request.ProfilePic = getUserDetails.ProfilePic;
                            if (getUserDetails.UpdatedBy != null)
                            {
                                var updatedUserDetails = await GetUserDetails(getUserDetails.UpdatedBy ?? 0);
                                if (updatedUserDetails != null)
                                {
                                    request.UpdatedBy = getUserDetails.UpdatedBy;
                                    request.UpdatedByName = updatedUserDetails.FirstName + " " + updatedUserDetails.LastName;
                                    request.UpdatedDate = DateUtil.GetDateString(getUserDetails.UpdatedDate);
                                }
                            }
                            else
                            {
                                var createdUserDetails = await GetUserDetails(getUserDetails.UpdatedBy ?? 0);
                                if (createdUserDetails != null)
                                {
                                    request.UpdatedBy = getUserDetails.UpdatedBy;
                                    request.UpdatedByName = createdUserDetails.FirstName + " " + createdUserDetails.LastName;
                                    request.UpdatedDate = DateUtil.GetDateString(getUserDetails.UpdatedDate);
                                }
                            }
                            return PartialView("~/Views/UserDetails/_UserDetailsForm.cshtml", request);
                        }
                        else
                        {
                            response.Acknowledge = Helper.AcknowledgeType.Failure;
                            response.Message = "User details not found";
                        }
                    }
                    else
                    {
                        return PartialView("~/Views/UserDetails/_UserDetailsForm.cshtml", request);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Acknowledge = Helper.AcknowledgeType.Failure;
                response.Message = "Failed to open user details form";
            }
            return Json(response);
        }

        public async Task<IActionResult> SaveUserDetals(CreateUpdateUserDetailRequest request)
        {
            BaseResponse response = new BaseResponse();
            if (request != null)
            {
                try
                {
                    using (var db = new CNJewellerDBContext())
                    {
                        if (request.Id > 0)
                        {
                            var getUserDetails = db.UserDetails.FirstOrDefault(x => x.Id == request.Id);
                            if (getUserDetails != null)
                            {
                                getUserDetails.OfficeId = request.OfficeId;
                                getUserDetails.FirstName = request.FirstName;
                                getUserDetails.MiddleName = request.MiddleName;
                                getUserDetails.LastName = request.LastName;
                                //getUserDetails.MobileNo = request.MobileNo;
                                getUserDetails.EmailId = request.EmailId;
                                getUserDetails.City = request.City;
                                getUserDetails.Pincode = request.Pincode;
                                getUserDetails.Address = request.Address;
                                getUserDetails.JoiningDate = DateUtil.GetDate(request.JoiningDate);
                                getUserDetails.RoleId = request.RoleId;
                                getUserDetails.UserType = request.UserType;
                                getUserDetails.Gender = request.Gender;
                                getUserDetails.ProfilePic = request.ProfilePic;
                                getUserDetails.UpdatedBy = CurrentUserId;
                                getUserDetails.UpdatedDate = CurrentDate;
                                await db.SaveChangesAsync();
                                response.Acknowledge = Helper.AcknowledgeType.Success;
                                response.Message = "Successfully updated user details";
                            }
                            else
                            {
                                response.Acknowledge = Helper.AcknowledgeType.Failure;
                                response.Message = "User details not found";
                            }
                        }
                        else
                        {

                            var passwordHasher = new PasswordHasher<string>();
                            var passwordHash = passwordHasher.HashPassword(request.MobileNo, request.Password);
                            string mobNo = request.MobileNo.Trim();
                            var mobileNoExist = db.UserDetails.FirstOrDefault(x => x.MobileNo.Trim() == mobNo); 
                            if (mobileNoExist != null)
                            {
                                response.Acknowledge = Helper.AcknowledgeType.Failure;
                                response.Message = "Mobile no already exists";
                            }
                            else
                            {
                                UserDetail userDetail = new UserDetail();
                                userDetail.OfficeId = request.OfficeId;
                                userDetail.FirstName = request.FirstName;
                                userDetail.MiddleName = request.MiddleName;
                                userDetail.LastName = request.LastName;
                                userDetail.MobileNo = request.MobileNo;
                                userDetail.EmailId = request.EmailId;
                                userDetail.City = request.City;
                                userDetail.Pincode = request.Pincode;
                                userDetail.Address = request.Address;
                                userDetail.JoiningDate = DateUtil.GetDate(request.JoiningDate);
                                userDetail.RoleId = request.RoleId;
                                userDetail.UserType = request.UserType;
                                userDetail.Gender = request.Gender;
                                userDetail.Password = passwordHash;
                                userDetail.ProfilePic = request.ProfilePic;
                                userDetail.CreatedBy = CurrentUserId;
                                userDetail.CreatedDate = CurrentDate;
                                userDetail.RowStatus = (int)RowStatus.Active;
                                await db.UserDetails.AddAsync(userDetail);
                                await db.SaveChangesAsync();
                                response.Acknowledge = Helper.AcknowledgeType.Success;
                                response.Message = "Successfully create new user";
                            }     
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Acknowledge = Helper.AcknowledgeType.Failure;
                    response.Message = ex.Message;
                }
            }
            else
            {
                response.Acknowledge = Helper.AcknowledgeType.Failure;
                response.Message = "Request is not valid";
            }
            return Json(response);
        }

        public async Task<IActionResult> UpdateUserStatus(int id, int statusId)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                using (var db = new CNJewellerDBContext())
                {
                    var recordStatus = statusId == 1 ? " Activated " : " InActivated ";
                    response = new BaseResponse
                    {
                        Acknowledge = AcknowledgeType.Success,
                        Message = "User " + recordStatus + " successfully"
                    };
                    var resultData = await db.UserDetails.FirstOrDefaultAsync(x => x.Id == id && x.RowStatus != (int)RowStatus.Deleted);
                    if (resultData != null)
                    {
                        resultData.RowStatus = statusId;
                        resultData.UpdatedBy = CurrentUserId;
                        resultData.UpdatedDate = DateUtil.GetCurrentDate();
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        response.Acknowledge = AcknowledgeType.Failure;
                        response.Message = "User does not exist";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Acknowledge = (int)AcknowledgeType.Failure;
                response.Message = ex.Message;
            }
            return Json(response);
        }

        public async Task<IActionResult> DeleteUserDetail(int id)
        {
            BaseResponse result = new BaseResponse();
            try
            {
                using (var db = new CNJewellerDBContext())
                {
                    var existingData = await db.UserDetails.FirstOrDefaultAsync(x => x.Id == id);
                    if (existingData != null)
                    {
                        existingData.RowStatus = (int)RowStatus.Deleted;
                        existingData.UpdatedBy = 1;
                        existingData.UpdatedDate = DateUtil.GetCurrentDate();
                        await db.SaveChangesAsync();
                        result = new BaseResponse()
                        {
                            Acknowledge = AcknowledgeType.Success,
                            Message = "User Deleted Succesfully"
                        };
                    }
                    result = new BaseResponse()
                    {
                        Acknowledge = AcknowledgeType.Failure,
                        Message = "User not exists"
                    };
                }
            }
            catch (Exception ex)
            {
                result.Acknowledge = (int)AcknowledgeType.Failure;
                result.Message = ex.Message;
            }
            return Json(result);
        }
    }
}
