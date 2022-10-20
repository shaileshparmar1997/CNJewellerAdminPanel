using CNJewellerAdmin.DTOs.Base;
using CNJewellerAdmin.DTOs.UserDetails;
using CNJewellerAdmin.Helper.DateUtil;
using CNJewellerAdmin.Helper;
using CNJewellerAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using CNJewellerAdmin.DTOs.Office;
using Microsoft.EntityFrameworkCore;
using System.Net;
using CNJewellerAdmin.Model;

namespace CNJewellerAdmin.Controllers
{
    public class OfficeMasterController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOfficeList(int statusId)
        {
            List<OfficeDetailsDTO> result = new List<OfficeDetailsDTO>();
            try
            {
                using (var db = new CNJContext())
                {
                    var userDetails = db.UserDetails.Where(x => x.RowStatus == (int)RowStatus.Active);
                    var officeDetails = db.OfficeMasters.Where(x => x.RowStatus == (int)RowStatus.Active).Select(x => new OfficeDetailsDTO
                    {
                        Id = x.Id,
                        OfficeName = x.OfficeName,
                        PhoneNo = x.PhoneNo,
                        Email = x.Email,
                        City = x.City,
                        Pincode = x.Pincode,
                        Address = x.Address,
                        BranchCode = x.BranchCode,
                        RowStatus = x.RowStatus
                    });
                    result = officeDetails.ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return Json(result);
        }

        public async Task<IActionResult> OpenOfficeDetailsForm(int id)
        {
            BaseResponse response = new BaseResponse();
            CreateUpdateOfficeMasterRequest request = new CreateUpdateOfficeMasterRequest();
            try
            {
                using (var db = new CNJContext())
                {
                    if (id>0)
                    {
                        var getOfficeDetails = await db.OfficeMasters.FirstOrDefaultAsync(x => x.Id == id && x.RowStatus == (int)RowStatus.Active);
                        if (getOfficeDetails != null)
                        {
                            request.Id = getOfficeDetails.Id;
                            request.OfficeName = getOfficeDetails.OfficeName;
                            request.PhoneNo = getOfficeDetails.PhoneNo;
                            request.Email = getOfficeDetails.Email;
                            request.City = getOfficeDetails.City;
                            request.Pincode = getOfficeDetails.Pincode;
                            request.Address = getOfficeDetails.Address;
                            request.BranchCode = getOfficeDetails.BranchCode;
                            if (getOfficeDetails.UpdatedBy != null)
                            {
                                var updatedUserDetails = await GetUserDetails(getOfficeDetails.UpdatedBy ?? 0);
                                if (updatedUserDetails != null)
                                {
                                    request.UpdatedBy = getOfficeDetails.UpdatedBy;
                                    request.UpdatedByName = updatedUserDetails.FirstName + " " + updatedUserDetails.LastName;
                                    request.UpdatedDate = DateUtil.GetDateString(getOfficeDetails.UpdatedDate);
                                }
                            }
                            else
                            {
                                var createdUserDetails = await GetUserDetails(getOfficeDetails.UpdatedBy ?? 0);
                                if (createdUserDetails != null)
                                {
                                    request.UpdatedBy = getOfficeDetails.UpdatedBy;
                                    request.UpdatedByName = createdUserDetails.FirstName + " " + createdUserDetails.LastName;
                                    request.UpdatedDate = DateUtil.GetDateString(getOfficeDetails.UpdatedDate);
                                }
                            }
                            return PartialView("~/Views/OfficeMaster/_OfficeDetailsForm.cshtml", request);
                        }
                    }
                    else
                    {
                        return PartialView("~/Views/OfficeMaster/_OfficeDetailsForm.cshtml", request);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Acknowledge = Helper.AcknowledgeType.Failure;
                response.Message = "Failed to open office details form";
            }
            return Json(response);
        }

        public async Task<IActionResult> SaveOfficeDetals(CreateUpdateOfficeMasterRequest request)
        {
            BaseResponse response = new BaseResponse();
            if (request != null)
            {
                try
                {
                    using (var db = new CNJContext())
                    {
                        if (request.Id > 0)
                        {
                            var getOfficeDetails = await db.OfficeMasters.FirstOrDefaultAsync(x => x.Id == request.Id);
                            if (getOfficeDetails != null)
                            {
                                getOfficeDetails.OfficeName = request.OfficeName;
                                getOfficeDetails.PhoneNo = request.PhoneNo;
                                getOfficeDetails.Email = request.Email;
                                getOfficeDetails.City = request.City;
                                getOfficeDetails.Pincode = request.Pincode;
                                getOfficeDetails.Address = request.Address;
                                getOfficeDetails.BranchCode = request.BranchCode;
                                getOfficeDetails.UpdatedBy = CurrentUserId;
                                getOfficeDetails.UpdatedDate = CurrentDate;
                                await db.SaveChangesAsync();
                                response.Acknowledge = Helper.AcknowledgeType.Success;
                                response.Message = "Office Updated Succesfully";
                            }
                            else
                            {
                                response.Acknowledge = Helper.AcknowledgeType.Failure;
                                response.Message = "Office details not found";
                            }
                        }
                        else
                        {
                            OfficeMaster officeMaster = new OfficeMaster();
                            officeMaster.OfficeName = request.OfficeName;
                            officeMaster.PhoneNo = request.PhoneNo;
                            officeMaster.Email = request.Email;
                            officeMaster.City = request.City;
                            officeMaster.Pincode = request.Pincode;
                            officeMaster.Address = request.Address;
                            officeMaster.BranchCode = request.BranchCode;
                            officeMaster.CreatedBy = CurrentUserId;
                            officeMaster.CreatedDate = CurrentDate;
                            officeMaster.RowStatus = (int)RowStatus.Active;
                            await db.OfficeMasters.AddAsync(officeMaster);
                            await db.SaveChangesAsync();
                            response.Acknowledge = Helper.AcknowledgeType.Success;
                            response.Message = "New Office Created Succesfully";
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

        public async Task<IActionResult> UpdateOfficeStatus(int id, int statusId)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                using (var db = new CNJContext())
                {
                    var recordStatus = statusId == 1 ? " Activated " : " InActivated ";
                    response = new BaseResponse
                    {
                        Acknowledge = AcknowledgeType.Success,
                        Message = "Office " + recordStatus + " successfully"
                    };
                    var resultData = await db.OfficeMasters.FirstOrDefaultAsync(x => x.Id == id && x.RowStatus != (int)RowStatus.Deleted);
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
                        response.Message = "Office does not exist";
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

        public async Task<IActionResult> DeleteOfficeDetail(int id)
        {
            BaseResponse result = new BaseResponse();
            try
            {
                using (var db = new CNJContext())
                {
                    var existingData = await db.OfficeMasters.FirstOrDefaultAsync(x => x.Id == id);
                    if (existingData != null)
                    {
                        existingData.RowStatus = (int)RowStatus.Deleted;
                        existingData.UpdatedBy = 1;
                        existingData.UpdatedDate = DateUtil.GetCurrentDate();
                        await db.SaveChangesAsync();
                        result = new BaseResponse()
                        {
                            Acknowledge = AcknowledgeType.Success,
                            Message = "Office Deleted Succesfully"
                        };
                    }
                    result = new BaseResponse()
                    {
                        Acknowledge = AcknowledgeType.Failure,
                        Message = "Office not exists"
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
