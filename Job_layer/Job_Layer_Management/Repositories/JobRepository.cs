using Job_Layer_Management.Controllers;
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
        private readonly ILogger<JOBController> Logger;

        public JobRepository(IConfiguration configuration, ILogger<JOBController> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(Configuration.GetConnectionString("JOBAppCon"));
        }



        // To get all job records
       public async Task<IEnumerable<JOB> > JobsGetAll()
       {
            Logger.LogInformation("Repository: Getting all jobs started.");

            var jobs = new List<JOB>();

            using var con = GetConnection();

           

            try
            {

                await con.OpenAsync();

                using var cmd = new SqlCommand("JobGetAll", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

              

                using var reader = await cmd.ExecuteReaderAsync();

               
                while (await reader.ReadAsync())
                {
                    jobs.Add(new JOB
                    {
                        JobID = (int)reader["JobID"],
                        JobName = reader["JobName"]?.ToString(),
                        JobDescription = reader["JobDescription"]?.ToString(),
                        JobType = reader["JobType"]?.ToString(),
                        StartDate = reader["StartDate"] as DateTime?,
                        EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],

                    });
                }
                Logger.LogInformation($"Repository: Getting all jobs completed. Total jobs: {jobs.Count}");


            }
            catch (SqlException sqlEx)

            {
                Logger.LogError("Repository:SQL Error: " + sqlEx.Message + sqlEx.StackTrace);


                await con.CloseAsync();

                throw (new Exception("SQL Error: " + sqlEx.Message + sqlEx.StackTrace));

            }



            return jobs;
        }



        // Retrieve job record by ID
        public async Task<IEnumerable<JOB>> GetJobById(int jobId)
        {
            Logger.LogInformation($"Repository: Getting job by ID {jobId} started.");

            var jobs = new List<JOB>();


            using var con = GetConnection();
            try
            {
                

                await con.OpenAsync();

                using var cmd = new SqlCommand("JobGet", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@JobID", jobId);

               

                using var reader = await cmd.ExecuteReaderAsync();


               

                while (await reader.ReadAsync())
                {
                    jobs.Add(new JOB
                    {
                        JobID = (int)reader["JobID"],
                        JobName = reader["JobName"]?.ToString(),
                        JobDescription = reader["JobDescription"]?.ToString(),
                        JobType = reader["JobType"]?.ToString(),
                        StartDate = reader["StartDate"] as DateTime?,
                        EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],

                    });
                }

                Logger.LogInformation($"Repository: Getting job by ID {jobId} completed.");
            }
            catch (SqlException sqlEx)

            {
                Logger.LogError("Repository: SQL Error: " + sqlEx.Message + sqlEx.StackTrace);

                await con.CloseAsync();

                throw (new Exception("SQL Error: " + sqlEx.Message + sqlEx.StackTrace));

            }
            return jobs;
        }




        // Delete the job record by ID 
        public async Task<(int, string, int? JobID)> JobDelete(int jobId)
        {
            Logger.LogInformation($"Repository: Deleting job with ID {jobId} started.");

            using var con = GetConnection();
            

            try
            {

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

                Logger.LogInformation($"Repository: Deleting job with ID {jobId} completed.");


                return (0, "No response from stored procedure", null);
            }
            catch (SqlException sqlEx)

            {

                Logger.LogError("Repository: SQL Error: " + sqlEx.Message + sqlEx.StackTrace);

                await con.CloseAsync();

                throw (new Exception("SQL Error: " + sqlEx.Message + sqlEx.StackTrace));

            }

        }


        // Update the job record with the given ID
        public async Task<(int ErrorCode, string Message)> UpdateJob(int jobId, JobUpdateDto jobDto)
        {
            Logger.LogInformation($"Repository: Updating job with ID {jobId} started.");

            using var con = GetConnection();
           
            try
            {
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

                Logger.LogInformation($"Repository: Updating job with ID {jobId} started.");

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    int errorCode = Convert.ToInt32(reader["ErrorCode"]);
                    string message = reader["Message"].ToString();

                    Logger.LogInformation($"Repository: Updating job with ID {jobId} completed with message: {message}");

                    return (errorCode, message);
                }

                Logger.LogWarning($"Repository: No result returned from stored procedure while updating job ID {jobId}");

                return (0, "No result returned from stored procedure.");
            }
            catch (SqlException sqlEx)

            {
                Logger.LogError("Repository: SQL Error: " + sqlEx.Message + sqlEx.StackTrace);

                await con.CloseAsync();

                throw (new Exception("SQL Error: " + sqlEx.Message + sqlEx.StackTrace));

            }
        }

        // Add a new job record using the provided information
        public async Task<(int ErrorCode, string Message, int? JobID)> AddJob( JobUpdateDto jobDto)
        {
            Logger.LogInformation("Repository: Adding new job started.");

            using var con = GetConnection();
           

            try
            {
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
                    
                    int errorCode = Convert.ToInt32(reader["ErrorCode"]);
                    string message = reader["Message"].ToString();
                    int? jobId = reader["JobID"] == DBNull.Value ? null : Convert.ToInt32(reader["JobID"]);
                    Logger.LogInformation($"Repository: Job added successfully. Job ID: {jobId}");
                    return (errorCode, message, jobId);
                }

                Logger.LogWarning("Repository: No result returned from stored procedure while adding job.");

                return (0, "No result returned from stored procedure", null);
            }
            catch (SqlException sqlEx)

            {
                Logger.LogError("Repository: SQL Error: " + sqlEx.Message + sqlEx.StackTrace);

                await con.CloseAsync();

                throw (new Exception("SQL Error: " + sqlEx.Message + sqlEx.StackTrace));

            }

        }


    }
}
