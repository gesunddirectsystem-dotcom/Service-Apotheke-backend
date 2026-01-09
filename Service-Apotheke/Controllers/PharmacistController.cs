using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Apotheke.Repository.Auth;
using Service_Apotheke.Repository.Pharmacist;
using ServiceApothekeAPI;
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
                    return Unauthorized("Invalid email or password");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
      

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdatePharmacistDto dto)
        {
            try
            {
                var result = await _pharmacistService.UpdateProfile(id, dto);
                if (!result)
                    return BadRequest("Pharmacist not found or update failed");

                return Ok("Profile updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("{id}/upload-cv")]
        public async Task<IActionResult> UploadCV(Guid id, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("لم يتم اختيار ملف");

            try
            {
                var result = await _pharmacistService.UploadCV(id, file);
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            try
            {
                var pharmacist = await _pharmacistService.GetById(id);
                if (pharmacist == null)
                    return NotFound("Pharmacist not found");

                // بنرجع بيانات الصيدلي كاملة عشان تظهر في البروفايل
                return Ok(pharmacist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("shifts/{shiftId}/complete")]
        public async Task<IActionResult> CompleteShift(Guid shiftId, [FromBody] CompleteShiftDto dto)
        {
            try
            {
                var result = await _pharmacistService.CompleteShift(shiftId, dto);
                if (!result) return NotFound("الشفت غير موجود");

                return Ok("تم إنهاء الشفت بنجاح وإضافة الملاحظات");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/notifications")]
        public async Task<IActionResult> GetNotifications(Guid id)
        {
            var notifications = await _pharmacistService.GetPharmacistNotifications(id);
            return Ok(notifications);
        }
    }
}