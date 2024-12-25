using Backend.DataAccess.Data.Requests;
using Backend.DataAccess.Data.Responses;
using Backend.DataAccess.UnitOfWork;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public PolicyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Policies")]
        public async Task<ActionResult<IEnumerable<InsurancePolicy>>> GetPolicies()
        {
            IEnumerable<InsurancePolicy> policies = await _unitOfWork.Policies.Get();
            if (policies is null)
                return NotFound();
            return Ok(policies);
        }

        [HttpGet("Policies/{policyNumber}")]
        public async Task<ActionResult<InsurancePolicy>> GetPolicy(string policyNumber)
        {
            InsurancePolicy insurancePolicy = await _unitOfWork.Policies.GetPolicyByPolicyNumber(policyNumber);
            if (insurancePolicy is null)
                return NotFound();
            return Ok(insurancePolicy);
        }

        [HttpPost("CreatePolicy")]
        public async Task<ActionResult<InsurancePolicy>> AddPolicy(InsurancePolicyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            InsurancePolicy insurancePolicy = await _unitOfWork.Policies.InsertPolicy(request);
            if (insurancePolicy is null)
                return NotFound();
            return Ok(insurancePolicy);
        }

        [HttpPost("CreatePolicyForPartner")]
        public async Task<ActionResult<PartnerResponse>> CreatePolicyForUser(string externalCode, InsurancePolicyRequest request)
        {
            if (string.IsNullOrEmpty(externalCode) || !ModelState.IsValid)
                return BadRequest();

            PartnerResponse partnerResponse = await _unitOfWork.Policies.CreatePolicyForPartner(request, externalCode);
            if (partnerResponse is null)
                return NotFound();
            return Ok(partnerResponse);
        }

        [HttpDelete("DeletePolicy/{id}")]
        public async Task<ActionResult<int>> DeletePolicy(int id)
        {
            if (id < 1)
                return BadRequest();

            int count = await _unitOfWork.Policies.Remove(id);
            if (count == 0)
                return NotFound();

            return Ok(count);
        }

        [HttpDelete("DeletePolicyByNumber/{policyNumber}")]
        public async Task<ActionResult<int>> DeletePolicyByNumber(string policyNumber)
        {
            if (string.IsNullOrEmpty(policyNumber))
                return BadRequest();

            int count = await _unitOfWork.Policies.RemovePolicyByPolicyNumber(policyNumber);
            if (count == 0)
                return NotFound();

            return Ok(count);
        }
    }
}
