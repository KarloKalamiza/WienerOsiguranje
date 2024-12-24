using Backend.DataAccess.Data.Responses;
using Backend.DataAccess.UnitOfWork;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PartnerController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    public PartnerController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("Partners")]   
    public async Task<ActionResult<IEnumerable<Partner>>> GetPartners()
    {
        IEnumerable<Partner> partners = await _unitOfWork.Partners.Get();
        if (partners is null)
            return NotFound();
        return Ok(partners);
    }

    [HttpGet("PartnersWithPolicies")]
    public async Task<ActionResult<IEnumerable<PartnerResponse>>> GetPartneWithPoliciess()
    {
        IEnumerable<PartnerResponse> partners = await _unitOfWork.Partners.GetPartnerWithPolicies();
        if (partners is null)
            return NotFound();
        return Ok(partners);
    }

    [HttpGet("PartnersWithPoliciesByID/{id}")]
    public async Task<ActionResult<PartnerResponse>> GetPartneWithPoliciessByID(int id)
    {
        PartnerResponse partner = await _unitOfWork.Partners.GetPartnerWithPoliciesById(id);
        if (partner is null)
            return NotFound();
        return Ok(partner);
    }

    [HttpGet("PartnersFilteredByPolicyNumber/{policyNumber}")]
    public async Task<ActionResult<IEnumerable<PartnerResponse>>> GetPartnersFilteredByPartnerNumber(string policyNumber)
    {
        IEnumerable<PartnerResponse> partners = await _unitOfWork.Partners.GetPartnersForPolicy(policyNumber);
        if (partners is null)
            return NotFound();
        return Ok(partners);
    }
}
