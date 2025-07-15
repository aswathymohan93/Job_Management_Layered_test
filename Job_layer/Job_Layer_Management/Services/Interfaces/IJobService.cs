using System.Collections.Generic;
using Job_Layer_Management.Models;

namespace Job_Layer_Management.Services.Interfaces
{
    public interface IJobService
    {
        // IEnumerable<JOB> GetAllJobs();

        Task<IEnumerable<JOB>> JobsGetAll();
        Task<IEnumerable<JOB>> GetJobById(int id);

        Task<(int ErrorCode, string Message, int? JobID)> JobDelete(int jobId);

        Task<(int ErrorCode, string Message)> UpdateJob(int jobId, JobUpdateDto jobDto);


        Task<(int ErrorCode, string Message, int? JobID)> AddJob( JobUpdateDto jobDto);

    }
}
