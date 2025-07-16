using Job_Layer_Management.Models;
using Job_Layer_Management.Repositories;
using Job_Layer_Management.Services.Interfaces;

namespace Job_Layer_Management.Services

{
    public class JobService : IJobService
    {
        private readonly JobRepository _repository;

        public JobService(JobRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<JOB>> JobsGetAll()
        {

            // To get all job records
            return await _repository.JobsGetAll();
        }
        public async Task<IEnumerable<JOB>> GetJobById(int id)
        {
            // Retrieve job record by ID
            return await _repository.GetJobById(id);
        }
        public async Task<(int ErrorCode, string Message,int? JobID)> JobDelete(int jobId)
        {
            // Delete the job record by ID 
            return await _repository.JobDelete(jobId);
        }
        public async Task<(int ErrorCode, string Message)> UpdateJob(int jobId, JobUpdateDto jobDto)
        {
            // Update the job record with the given ID
            return await _repository.UpdateJob(jobId, jobDto);
        }

        public async Task<(int ErrorCode, string Message, int? JobID)> AddJob( JobUpdateDto jobDto)
        {
            // Add a new job record using the provided information
            return await _repository.AddJob( jobDto);
        }

    }
}
