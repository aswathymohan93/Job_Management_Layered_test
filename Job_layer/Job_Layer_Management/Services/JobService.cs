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
            return await _repository.JobsGetAll();
        }
        public async Task<IEnumerable<JOB>> GetJobById(int id)
        {
            return await _repository.GetJobById(id);
        }
        public async Task<(int ErrorCode, string Message,int? JobID)> JobDelete(int jobId)
        {
            return await _repository.JobDelete(jobId);
        }
        public async Task<(int ErrorCode, string Message)> UpdateJob(int jobId, JobUpdateDto jobDto)
        {
            return await _repository.UpdateJob(jobId, jobDto);
        }

        public async Task<(int ErrorCode, string Message, int? JobID)> AddJob( JobUpdateDto jobDto)
        {
            return await _repository.AddJob( jobDto);
        }

    }
}
