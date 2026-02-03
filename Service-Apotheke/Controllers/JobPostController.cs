using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service_Apotheke.Repository.Job;
using ServiceApothekeAPI;

namespace ServiceApothekeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobPostController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobPost([FromBody] CreateJobPostDto dto)
        {
            try
            {
                var result = await _jobService.CreateJobPost(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("pharmacy/{pharmacyId}")]
        public async Task<IActionResult> GetPharmacyJobPosts(Guid pharmacyId)
        {
            try
            {
                var result = await _jobService.GetPharmacyJobPosts(pharmacyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("applicant/{pharmacistId}")]
        public async Task<IActionResult> GetPharmacistAppliedJobs(Guid pharmacistId)
        {
            try
            {
                var result = await _jobService.GetPharmacistAppliedJobs(pharmacistId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobPostById(Guid id)
        {
            try
            {
                var result = await _jobService.GetJobPostById(id);
                if (result == null) return NotFound("Schicht nicht gefunden");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobPostStatus(Guid id, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                var result = await _jobService.UpdateJobPostStatus(id, dto.Status);

                if (!result) return NotFound("Schicht nicht gefunden");

                return Ok("Status erfolgreich aktualisiert");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveJobPosts()
        {
            try
            {
                var result = await _jobService.GetActiveJobPosts();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}