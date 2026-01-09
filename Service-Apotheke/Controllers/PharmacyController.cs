using Microsoft.AspNetCore.Mvc;
using Service_Apotheke;
using Service_Apotheke.Repository.Auth;
using Service_Apotheke.Repository.Pharmacy;
using ServiceApothekeAPI;
namespace ServiceApothekeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _pharmacyService;
        private readonly IAuthService _authService;

        public PharmacyController(IPharmacyService pharmacyService, IAuthService authService)
        {
            _pharmacyService = pharmacyService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] PharmacyRegDto dto)
        {
            try
            {
                var result = await _authService.RegisterPharmacy(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyDto dto)
        {
            try
            {
                var result = await _authService.VerifyPharmacyEmail(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginPharmacy(dto);
                if (result == null)
                    return Unauthorized("Invalid email or password");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/full-details")]
        public async Task<IActionResult> GetPharmacyFullDetails(Guid id)
        {
            var result = await _pharmacyService.GetPharmacyFullDetails(id);
            if (result == null) return NotFound("Pharmacy not found");
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePharmacy(Guid id, [FromBody] UpdatePharmacyDto dto)
        {
            try
            {
                var result = await _pharmacyService.UpdatePharmacy(id, dto);
                if (!result)
                    return BadRequest("Pharmacy not found or update failed");

                return Ok("Pharmacy updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}