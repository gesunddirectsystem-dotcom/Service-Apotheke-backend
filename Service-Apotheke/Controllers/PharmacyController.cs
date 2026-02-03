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
                    return Unauthorized("Ungültige E-Mail oder Passwort");

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
            if (result == null) return NotFound("Apotheke nicht gefunden");
            return Ok(result);
        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {
            try
            {
                var result = await _authService.ForgetPharmacyPassword(dto.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                var result = await _authService.ResetPharmacyPassword(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePharmacy(Guid id, [FromBody] UpdatePharmacyDto dto)
        {
            try
            {
                var result = await _pharmacyService.UpdatePharmacy(id, dto);
                if (!result)
                    return BadRequest("Apotheke nicht gefunden oder Aktualisierung fehlgeschlagen");

                return Ok("Apotheke erfolgreich aktualisiert");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}