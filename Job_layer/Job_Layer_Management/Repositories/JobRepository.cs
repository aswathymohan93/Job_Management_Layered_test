using Job_Layer_Management.Models;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Transactions;

namespace Job_Layer_Management.Repositories
{
    public class JobRepository
    {
        private readonly IConfiguration Configuration;

        public JobRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(Configuration.GetConnectionString("JOBAppCon"));
        }

      public  async Task<IEnumerable<JOB> > JobsGetAll()
        {
            var jobs = new List<JOB>();

            using var con = GetConnection();

            await con.OpenAsync(); 

            using var cmd = new SqlCommand("JobGetAll", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) // ✅ Async reading
            {
                jobs.Add(new JOB
                {
                    JobID = (int)reader["JobID"],
                    JobName = reader["JobName"]?.ToString(),
                    JobDescription = reader["JobDescription"]?.ToString(),
                    JobType = reader["JobType"]?.ToString(),
                    StartDate = reader["StartDate"] as DateTime?,
                    EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
                  //  CreatedOn = (DateTime)reader["CreatedOn"],
                   // UpdatedOn = reader["UpdatedOn"] == DBNull.Value ? null : (DateTime?)reader["UpdatedOn"]
                });
            }

            return jobs;
        }

        public async Task<IEnumerable<JOB>> GetJobById(int jobId)
        {
            var jobs = new List<JOB>();

            using var con = GetConnection();

            await con.OpenAsync();

            using var cmd = new SqlCommand("JobGet", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@JobID", jobId);


            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) // ✅ Async reading
            {
                jobs.Add(new JOB
                {
                    JobID = (int)reader["JobID"],
                    JobName = reader["JobName"]?.ToString(),
                    JobDescription = reader["JobDescription"]?.ToString(),
                    JobType = reader["JobType"]?.ToString(),
                    StartDate = reader["StartDate"] as DateTime?,
                    EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
                  //  CreatedOn = (DateTime)reader["CreatedOn"],
                    //UpdatedOn = reader["UpdatedOn"] == DBNull.Value ? null : (DateTime?)reader["UpdatedOn"]
                });
            }

            return jobs;
        }

        public async Task<(int, string, int? JobID)> JobDelete(int jobId)
        {
            using var con = GetConnection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("JobDelete", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@JobID", jobId);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                int errorCode = Convert.ToInt32(reader["ErrorCode"]);
                string message = reader["Message"].ToString();
                return (errorCode, message, jobId);
            }

            return (0, "No response from stored procedure",null);
        }

        public async Task<(int ErrorCode, string Message)> UpdateJob(int jobId, JobUpdateDto jobDto)
        {
            using var con = GetConnection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("JobUpdate", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@JobID", jobId);
            cmd.Parameters.AddWithValue("@JobName", jobDto.JobName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@JobDescription", jobDto.JobDescription ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@JobType", jobDto.JobType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@StartDate", jobDto.StartDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDate", jobDto.EndDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", jobDto.IsActive);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                int errorCode = Convert.ToInt32(reader["ErrorCode"]);
                string message = reader["Message"].ToString();
                return (errorCode, message);
            }

            return (0, "No result returned from stored procedure.");
        }

        public async Task<(int ErrorCode, string Message, int? JobID)> AddJob( JobUpdateDto jobDto)
        {
            using var con = GetConnection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("JobAdd", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            int insertedId = 0;

            cmd.Parameters.AddWithValue("@JobName", jobDto.JobName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@JobDescription", jobDto.JobDescription ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@JobType", jobDto.JobType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@StartDate", jobDto.StartDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDate", jobDto.EndDate ?? (object)DBNull.Value);
            

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                //int errorCode = Convert.ToInt32(reader["ErrorCode"]);
                //string message = reader["Message"].ToString();
                //return (errorCode, message);
                int errorCode = Convert.ToInt32(reader["ErrorCode"]);
                string message = reader["Message"].ToString();
                int? jobId = reader["JobID"] == DBNull.Value ? null : Convert.ToInt32(reader["JobID"]);
                return (errorCode, message, jobId);
            }

            return (0, "No result returned from stored procedure", null);
        }


    }
}
