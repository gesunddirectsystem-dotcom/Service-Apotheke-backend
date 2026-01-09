using System.ComponentModel.DataAnnotations;

    public class CreateJobPostDto
    {
        [Required]
        public Guid PharmacyId { get; set; }

        [Required]
        public DateTime ShiftDate { get; set; }

        [Required]
        public string StartTime { get; set; } = string.Empty;

        [Required]
        public string EndTime { get; set; } = string.Empty;

        public bool HasBreak { get; set; }

        public int? BreakDurationInMinutes { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal Salary { get; set; }

        public bool HasTransportationAllowance { get; set; }

        public decimal? TransportationAllowanceAmount { get; set; }

        public string Notes { get; set; } = string.Empty;
    }
public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty; 
}
public class ApplyDto
    {
        [Required]
        public Guid PharmacistId { get; set; }

        [Required]
        public Guid JobPostId { get; set; }
    }

    public class RespondDto
    {
        [Required]
        public Guid ApplicationId { get; set; }

        [Required]
        public string NewStatus { get; set; } = string.Empty; // "Accepted" or "Rejected"
    }