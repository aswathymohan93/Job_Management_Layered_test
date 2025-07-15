using Job_Layer_Management.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Job_Layer_Management.Models;


namespace Job_Layer_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JOBController : ControllerBase
    {

        private readonly IJobService _jobService;

        public JOBController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs()
        {
            try
            { // get all jobs
                var jobs = await _jobService.JobsGetAll();

                // return Ok(jobs.Count()+" number of records found");

                return Ok(new
                {
                    count = jobs.Count() + " number of records found",

                    data = jobs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            try
            { // get  job by id
                var jobs = await _jobService.GetJobById(id);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            //var result = await _jobService.JobDelete(id);

            //int errorCode = result.ErrorCode;
            //string message = result.Message;
            //int jobId = result.jobId;

          
            var (errorCode, message, jobId) = await _jobService.JobDelete(id);

            return errorCode switch
            {
                1 => Ok(new { message, jobId }),
                -1 or -2 => NotFound(new { message, jobId }),
                _ => StatusCode(500, new { message = "Unknown error occurred.", jobId })
            };
        }

      

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobUpdateDto jobDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(jobDto.JobName))
                return BadRequest("Job name is required.");

            if (string.IsNullOrWhiteSpace(jobDto.JobDescription))
                return BadRequest("Job description is required.");

            if (string.IsNullOrWhiteSpace(jobDto.JobType))
                return BadRequest("Job type is required.");

            if (jobDto.StartDate == null)
                return BadRequest("Start date is required.");

            if (jobDto.EndDate == null)
                return BadRequest("End date is required.");

            if (jobDto.StartDate > jobDto.EndDate)
                return BadRequest("Start date cannot be later than end date.");

            var (errorCode, message) = await _jobService.UpdateJob(id, jobDto);

            return errorCode switch
            {
                -1 => Conflict(message),
                -2 => NotFound(message),
                -3 => NotFound(message),
                1 => Ok(message),
                _ => StatusCode(500, "Unknown error.")
            };
        }

        [HttpPost("Add")]
        public async Task<IActionResult>AddJob( [FromBody] JobUpdateDto jobDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(jobDto.JobName))
                return BadRequest("Job name is required.");

            if (string.IsNullOrWhiteSpace(jobDto.JobDescription))
                return BadRequest("Job description is required.");

            if (string.IsNullOrWhiteSpace(jobDto.JobType))
                return BadRequest("Job type is required.");

            if (jobDto.StartDate == null)
                return BadRequest("Start date is required.");

            if (jobDto.EndDate == null)
                return BadRequest("End date is required.");

            if (jobDto.StartDate > jobDto.EndDate)
                return BadRequest("Start date cannot be later than end date.");

            var (errorCode, message, jobId) = await _jobService.AddJob(jobDto);

            return errorCode switch
            {
                -1 => Conflict(new { message }),
                1 => Ok(new { message, jobId }),
                _ => StatusCode(500, "Unknown error.")
            };
        }


    }
}
