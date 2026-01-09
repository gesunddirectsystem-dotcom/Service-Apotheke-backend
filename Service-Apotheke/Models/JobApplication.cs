using Service_Apotheke.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class JobApplication
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid JobPostId { get; set; }

    [Required]
    public Guid PharmacistId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("JobPostId")]
    public JobPost JobPost { get; set; } = null!;

    [ForeignKey("PharmacistId")]
    public Pharmacist Pharmacist { get; set; } = null!;
}