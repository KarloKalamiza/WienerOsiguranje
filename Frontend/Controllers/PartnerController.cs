using Frontend.DTO;
using Frontend.Mappers;
using Frontend.Models;
using Frontend.Request;
using Frontend.Responses;
using Frontend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers;

public class PartnerController : Controller
{
    private readonly CrmService _crmService;

    public PartnerController(CrmService crmService)
    {
        _crmService = crmService;
    }

    public async Task<ActionResult> Index(int? newId = null)
    {
        ServiceResponse serviceResponse = await _crmService.GetPartners();

        List<PartnerDTO>? partnerDTOs = serviceResponse.Data as List<PartnerDTO>;
        ViewBag.NewId = newId;
        return View(partnerDTOs);
    }

    // GET: PartnerController/Details/5
    public async Task<ActionResult> Details(int id)
    {
        ServiceResponse serviceResponse = await _crmService.GetById(id);
        if (serviceResponse != null && serviceResponse.Success)
        {
            if (serviceResponse.Data is not PartnerDTO partner)
            {
                return NotFound();
            }

            return View("Details", partner);
        }
        else
        {
            return NotFound();
        }
    }

    // GET: PartnerController/Create
    public ActionResult<PartnerRequest> Create()
    {
        return View();
    }

    // POST: PartnerController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(PartnerRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            ServiceResponse response = await _crmService.CreatePartner(request);

            if (!response.Success)
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage);
                return View(request);
            }

            if (response.Data is PartnerDTO createdPartner)
            {
                return RedirectToAction(nameof(Index), new { newId = createdPartner.PartnerId });
            }

            ModelState.AddModelError(string.Empty, "Failed to extract partner ID.");
            return View(request);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
            return View(request);
        }
    }

    // GET: PartnerController/Edit/5
    public async Task<ActionResult> Edit(int id)
    {
        try
        {
            ServiceResponse serviceResponse = await _crmService.FindPartnerByID(id);
            if (serviceResponse != null && serviceResponse.Success)
            {
                Partner? partner = serviceResponse.Data as Partner ?? new Partner();
                EditPartnerDTO editPartnerDTO = PartnerMapper.MapToEditPartnerDTO(partner);
                return View(editPartnerDTO);
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to fetch partner details.";

                return View();
            }
        }
        catch
        {
            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";

            return RedirectToAction("Index", "Partner");
        }
    }

    // POST: PartnerController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(int id, EditPartnerDTO partner)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Validation failed. Please correct the errors and try again.";
            return View(partner); 
        }

        try
        {
            ServiceResponse serviceResponse = await _crmService.UpdatePartner(id, partner);
            if (serviceResponse != null && serviceResponse.Success)
            {
                TempData["SuccessMessage"] = "Partner updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errorMessage = serviceResponse?.ErrorMessage ?? "Unknown error occurred.";
                TempData["ErrorMessage"] = $"Update failed: {errorMessage}";
                return View(partner); 
            }
        }
        catch 
        {
            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
            return View(partner); 
        }
    }
}
