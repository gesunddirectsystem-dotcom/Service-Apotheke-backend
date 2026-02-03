using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service_Apotheke.Repository.Auth;
using Service_Apotheke.Repository.Pharmacist;
using System;
using System.Threading.Tasks;

namespace ServiceApothekeAPI.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacistController : ControllerBase
    {
        private readonly IPharmacistService _pharmacistService;
        private readonly IAuthService _authService;

        public PharmacistController(IPharmacistService pharmacistService, IAuthService authService)
        {
            _pharmacistService = pharmacistService;
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] PharmacistRegDto dto)
        {
            try
            {
                var result = await _authService.RegisterPharmacist(dto);
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
                var result = await _authService.VerifyPharmacistEmail(dto);
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
                var result = await _authService.LoginPharmacist(dto);
                if (result == null)
                    return Unauthorized("Ungültige E-Mail oder Passwort");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile([FromRoute] Guid id, [FromBody] UpdatePharmacistDto dto)
        {
            try
            {
                var result = await _pharmacistService.UpdateProfile(id, dto);
                if (!result)
                    return BadRequest("Apotheker nicht gefunden oder Aktualisierung fehlgeschlagen");

                return Ok("Profil erfolgreich aktualisiert");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/upload-cv")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> UploadCV([FromRoute] Guid id, [FromForm] UploadCvDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Keine Datei ausgewählt");

            try
            {
                var result = await _pharmacistService.UploadCV(id, dto.File);
                return Ok(new { path = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/all-shifts")]
        public async Task<IActionResult> GetAllShifts(Guid id)
        {
            var shifts = await _pharmacistService.GetAllShifts(id);
            return Ok(shifts);
        }

        [HttpGet("{id}/upcoming-shifts")]
        public async Task<IActionResult> GetUpcomingShifts(Guid id)
        {
            var shifts = await _pharmacistService.GetUpcomingShifts(id);
            return Ok(shifts);
        }

        [HttpGet("{id}/completed-shifts")]
        public async Task<IActionResult> GetCompletedShifts(Guid id)
        {
            var shifts = await _pharmacistService.GetCompletedShifts(id);
            return Ok(shifts);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            try
            {
                var pharmacist = await _pharmacistService.GetById(id);
                if (pharmacist == null)
                    return NotFound("Apotheker nicht gefunden");

                return Ok(pharmacist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {
            try
            {
                var result = await _authService.ForgetPharmacistPassword(dto.Email);
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
                var result = await _authService.ResetPharmacistPassword(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("shifts/{shiftId}/complete")]
        public async Task<IActionResult> CompleteShift(Guid shiftId, [FromBody] CompleteShiftDto dto)
        {
            try
            {
                var result = await _pharmacistService.CompleteShift(shiftId, dto);
                if (!result) return NotFound("Schicht nicht gefunden");

                return Ok("Schicht erfolgreich abgeschlossen und Notizen hinzugefügt");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("{id}/notifications")]
        public async Task<IActionResult> GetNotifications(Guid id)
        {
            var notifications = await _pharmacistService.GetPharmacistNotifications(id);
            return Ok(notifications);
        }
    }
}