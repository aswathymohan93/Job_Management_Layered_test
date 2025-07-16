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
        private readonly ILogger<JOBController> Logger;

        private readonly IJobService JobService;

        public JOBController(IJobService jobService, ILogger<JOBController> logger)
        {
            JobService = jobService;
            Logger = logger;

        }

        /// <summary>
        /// Retrieves all job records from the system.
        /// </summary>
        /// <returns>
        /// A response containing the total count of job records and the job data.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetJobs() 
        {
            Logger.LogInformation("Getting all jobs started.");

            try
            {
                // To get all job records
                var jobs = await JobService.JobsGetAll();


                Logger.LogInformation($"Getting all jobs completed. Total jobs: {jobs.Count()}");

                return Ok(new
                {
                    count = jobs.Count() + " number of records found",

                    data = jobs
                });
            }
            catch (Exception ex)
            {

                Logger.LogError("Exception" + ex.Message);
                throw (new Exception("Exception" + ex.Message));

                
            }
        }

        /// <summary>
        /// Retrieves a specific job record based on the provided job ID.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the job to retrieve. This is a required route parameter; if not provided, a validation error will occur.
        /// </param>
        /// <returns>The job record corresponding to the specified job ID.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            try
            {
                Logger.LogInformation($"Getting job by ID {id} started.");
                // Retrieve job record by ID
                var jobs = await JobService.GetJobById(id);

                Logger.LogInformation($"Getting job by ID {id} completed.");

                return Ok(jobs);

            }
            catch (Exception ex)
            {
                Logger.LogError("Exception" + ex.Message);
                throw (new Exception("Exception" + ex.Message));
                
            }
        }

        /// <summary>
        /// Deletes a job record based on the provided job ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the job to delete. This is required; if not provided, a validation error will occur.
        /// </param>
        /// <returns>
        /// A response indicating whether the job was successfully deleted, not found, or if an error occurred.
        /// </returns>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {

            try
            {
                Logger.LogInformation($"Deleting job with ID {id} started.");
                // Delete the job record by ID 
                var (errorCode, message, jobId) = await JobService.JobDelete(id);

                Logger.LogInformation($"Deleting job with ID {id} completed.");

                return errorCode switch
                {
                    1 => Ok(new { message, jobId }),
                    -1 or -2 => NotFound(new { message, jobId }),
                    _ => StatusCode(500, new { message = "Unknown error occurred.", jobId })
                };
            }
            catch (Exception ex)
            {

                Logger.LogError("Exception" + ex.Message);
                throw (new Exception("Exception" + ex.Message));


            }

        }


        /// <summary>
        /// Updates an existing job record based on the provided job ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the job to update. This is required; if not provided, a validation error will occur.
        /// </param>
        /// <returns>
        /// A response indicating whether the job was successfully updated, not found, already in use, or if an error occurred.
        /// </returns>
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

            try
            {

                Logger.LogInformation($"Updating job with ID {id} started.");
                // Update the job record with the given ID
                var (errorCode, message) = await JobService.UpdateJob(id, jobDto); // try catch 

                Logger.LogInformation($"Updating job with ID {id} completed.");

                return errorCode switch
                {
                    -1 => Conflict(message),
                    -2 => NotFound(message),
                    -3 => NotFound(message),
                    1 => Ok(message),
                    _ => StatusCode(500, "Unknown error.")
                };
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception" + ex.Message);
                throw (new Exception("Exception" + ex.Message));


            }

        }


        /// <summary>
        /// Adds a new job record to the system.
        /// </summary>
        /// <returns>Returns the outcome of the add operation, including the newly created job ID if successful.
        /// </returns>

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

            try
            {
                Logger.LogInformation("Adding new job started.");
                // Add a new job record using the provided information
                var (errorCode, message, jobId) = await JobService.AddJob(jobDto);

                Logger.LogInformation($"Adding new job completed. Job ID: {jobId}");

                return errorCode switch
                {
                    -1 => Conflict(new { message }),
                    1 => Ok(new { message, jobId }),
                    _ => StatusCode(500, "Unknown error.")
                };
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception" + ex.Message);
                throw (new Exception("Exception" + ex.Message));


            }


        }


    }
}
