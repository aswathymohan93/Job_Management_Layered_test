using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;


namespace Job_Layer_Management.Models
{
    public class JOB
    {
        public int JobID { get; set; }

        [Required]
        [StringLength(100)]
        public string? JobName { get; set; }
        public string? JobDescription { get; set; }
        public string? JobType { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; }
        //public bool IsDeleted { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime CreatedOn { get; set; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime? UpdatedOn { get; set; }
    }
    public class JobUpdateDto
    {

        public string JobName { get; set; }
        public string JobDescription { get; set; }
        public string JobType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

    }



}
