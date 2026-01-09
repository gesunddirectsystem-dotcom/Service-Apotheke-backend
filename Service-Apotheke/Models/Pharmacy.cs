using ServiceApothekeAPI;
using System.ComponentModel.DataAnnotations;
namespace Service_Apotheke.Models
{

    public class Pharmacy
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string PharmacyName { get; set; } = string.Empty;

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

        [Required]
        [MaxLength(100)]
        public string LicenseNumber { get; set; } = string.Empty;

        public bool IsVerified { get; set; } = false;

        [MaxLength(10)]
        public string ConfirmationCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
    }
}