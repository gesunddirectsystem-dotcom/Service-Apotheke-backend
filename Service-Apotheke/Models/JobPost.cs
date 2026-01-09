using Service_Apotheke.Models;
using ServiceApothekeAPI;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class JobPost
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid PharmacyId { get; set; }

    [Required]
    public DateTime ShiftDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string StartTime { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string EndTime { get; set; } = string.Empty;

    public bool HasBreak { get; set; }

    public int? BreakDurationInMinutes { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Salary { get; set; }

    public bool HasTransportationAllowance { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? TransportationAllowanceAmount { get; set; }

    public string Notes { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("PharmacyId")]
    public Pharmacy Pharmacy { get; set; } = null!;
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}