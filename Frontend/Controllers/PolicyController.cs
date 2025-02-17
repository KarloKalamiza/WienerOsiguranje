﻿using Frontend.DTO;
using Frontend.Mappers;
using Frontend.Models;
using Frontend.Request;
using Frontend.Responses;
using Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class PolicyController : Controller
    {
        private readonly CrmService _crmService;
        private static string _partnerExternalCode = string.Empty;
        private static string _partnerid = string.Empty;
        public PolicyController(CrmService crmService)
        {
            _crmService = crmService;
        }

        // GET: PolicyController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PolicyController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PolicyController/Create
        public ActionResult<InsurancePolicyRequest> Create(string externalCode, int partnerID)
        {
            _partnerid = partnerID.ToString() ?? string.Empty;
            _partnerExternalCode = externalCode ?? string.Empty;
            return View();
        }

        // POST: PolicyController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InsurancePolicyRequest policy)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(policy);
                }

                ServiceResponse response = await _crmService.CreatePolicyForPartner(_partnerExternalCode, policy);

                if (!response.Success)
                {
                    ModelState.AddModelError(string.Empty, response.ErrorMessage);
                    return View(policy);
                }

                if (response.Data is InsurancePolicy)
                {
                    return RedirectToAction("Details", "Partner", new { id = _partnerid });
                }


                ModelState.AddModelError(string.Empty, "Failed to extract policy ID.");
                return View(policy);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
                return View(policy);
            }
        }

        // GET: PolicyController/Edit/5
        public async Task<ActionResult> Edit(string policyNumber, int partnerId)
        {
            try
            {
                _partnerid = partnerId.ToString() ?? string.Empty;
                ServiceResponse serviceResponse = await _crmService.FindPolicyByPolicyNumber(policyNumber);

                if (serviceResponse == null || !serviceResponse.Success || serviceResponse.Data == null)
                {
                    TempData["ErrorMessage"] = "Policy not found or an error occurred while retrieving the policy.";
                    return View(); 
                }

                InsurancePolicy? fetchedInsurancePolicy = serviceResponse.Data as InsurancePolicy;

                if (fetchedInsurancePolicy == null)
                {
                    TempData["ErrorMessage"] = "Invalid policy data retrieved.";
                    return View();
                }

                EditInsurancePolicyDTO editInsurancePolicyDTO = InsurancePolicyMapper.MapToEditInsurancePolicyDTO(fetchedInsurancePolicy);

                return View(editInsurancePolicyDTO);
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";

                return View();
            }
        }

        // POST: PolicyController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, EditInsurancePolicyDTO request)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "There are validation errors. Please check your input.";
                return View(request);
            }
            try
            {
                ServiceResponse serviceResponse = await _crmService.UpdatePolicy(id, request);

                if (serviceResponse.Success)
                {
                    return RedirectToAction("Details", "Partner", new { id = _partnerid });
                }
                else
                {
                    TempData["ErrorMessage"] = serviceResponse.ErrorMessage;
                    return View(request);
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Error occurred while updating a policy.";
                return View(request);
            }
        }

        // POST: PolicyController/Delete/5
        [HttpDelete("api/Policy/DeletePolicy/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                ServiceResponse serviceResponse = await _crmService.DeletePolicy(id);
                
                if (serviceResponse.Success)
                {
                    return Json(new { success = true, message = "Policy deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Policy could not be deleted." });
                }
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred while deleting the policy." });
            }
        }
    }
}
