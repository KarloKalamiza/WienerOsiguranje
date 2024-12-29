using Frontend.DTO;
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

    // GET: PartnerController
    //public async Task<ActionResult> Index()
    //{
    //    List<DTO.PartnerDTO> partnerDTOs = await _crmService.GetPartners();
    //    return View(partnerDTOs);
    //}

    public async Task<ActionResult> Index(int? newId = null)
    {
        List<DTO.PartnerDTO> partnerDTOs = await _crmService.GetPartners();
        PartnerDTO? partnerDTO = partnerDTOs.FirstOrDefault();
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
    public ActionResult Edit(int id)
    {
        return View();
    }

    // POST: PartnerController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: PartnerController/Delete/5
    public ActionResult Delete(int id)
    {
        return View();
    }

    // POST: PartnerController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}
