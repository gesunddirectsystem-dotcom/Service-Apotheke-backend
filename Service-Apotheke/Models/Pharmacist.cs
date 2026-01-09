using ServiceApothekeAPI;
using System.ComponentModel.DataAnnotations;
namespace Service_Apotheke.Models
{
    public class Pharmacist
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public string GraduationCertificatePath { get; set; } = string.Empty;

        public string CvPath { get; set; } = string.Empty;

        public bool HasTransportation { get; set; }

        [MaxLength(50)]
        public string TransportationType { get; set; } = string.Empty;

        public bool HasDrivingLicense { get; set; }

        [MaxLength(50)]
        public string TaxNumber { get; set; } = string.Empty;

        public int MaxDistanceKm { get; set; }

        public int AvailableDaysPerWeek { get; set; }

        public bool IsProfileCompleted { get; set; } = false;

        public bool IsEmailConfirmed { get; set; } = false;

        [MaxLength(10)]
        public string ConfirmationCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
        public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
    }
}