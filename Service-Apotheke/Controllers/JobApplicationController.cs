using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service_Apotheke.Repository.Job;
using ServiceApothekeAPI;

namespace ServiceApothekeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobApplicationController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForShift([FromBody] ApplyDto dto)
        {
            try
            {
                var result = await _jobService.ApplyForShift(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("respond")]
        public async Task<IActionResult> RespondToApplication([FromBody] RespondDto dto)
        {
            try
            {
                var result = await _jobService.RespondToApplication(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("pharmacist/{pharmacistId}")]
        public async Task<IActionResult> GetPharmacistApplications(Guid pharmacistId)
        {
            try
            {
                var result = await _jobService.GetPharmacistApplications(pharmacistId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("jobpost/{jobPostId}")]
        public async Task<IActionResult> GetJobPostApplications(Guid jobPostId)
        {
            try
            {
                var result = await _jobService.GetJobPostApplications(jobPostId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}