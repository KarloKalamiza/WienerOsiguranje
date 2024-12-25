using Backend.DataAccess.Data.Requests;
using Backend.DataAccess.Data.Responses;
using Backend.DataAccess.UnitOfWork;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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
        if (string.IsNullOrEmpty(policyNumber))
            return NotFound("Policy number is required");

        IEnumerable<PartnerResponse> partners = await _unitOfWork.Partners.GetPartnersForPolicy(policyNumber);
        if (partners is null)
            return NotFound();
        return Ok(partners);
    }

    [HttpPost("CreatePartner")]
    public async Task<ActionResult<Partner>> CreatePartner([FromBody] PartnerRequest partnerRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Partner partner = await _unitOfWork.Partners.InsertPartner(partnerRequest);
        if (partner is null)
            return BadRequest();
        return Ok(partner);
    }

    [HttpDelete("DeletePartner/{id}")]
    public async Task<ActionResult<int>> DeletePartner(int id)
    {
        if (id < 1)
            return BadRequest();

        int count = await _unitOfWork.Partners.Remove(id);
        if (count == 0)
            return NotFound();

        return Ok(count);
    }

    [HttpDelete("SoftDeleteUser/{externalCode}")] 
    public async Task<ActionResult<int>> SoftDeletePartner(string externalCode, [FromQuery] string deletedByUser)
    {
        if (string.IsNullOrEmpty(externalCode))
            return BadRequest("External code is required");

        if (string.IsNullOrEmpty(deletedByUser) || !IsValidEmail(deletedByUser))
            return BadRequest("DeletedByUser is required and in email valid format");

        int count = await _unitOfWork.Partners.SoftDeletePartner(externalCode, deletedByUser);
        if (count == 0)
            return NotFound();

        return Ok(count);
    }

    private bool IsValidEmail(string email)
    {
        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailRegex);
    }
}
