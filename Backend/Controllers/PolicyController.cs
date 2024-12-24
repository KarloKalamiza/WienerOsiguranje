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
    }
}
