using System.ComponentModel.DataAnnotations;

public class CompleteShiftDto
{
    public string PharmacistNotes { get; set; } = string.Empty;
}
public class PharmacistLoginResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsProfileCompleted { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
    public class PharmacistRegDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
    }

  

    public class UpdatePharmacistDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        [Required]
        public bool HasTransportation { get; set; }

        public string TransportationType { get; set; } = string.Empty;

        [Required]
        public bool HasDrivingLicense { get; set; }

        public string TaxNumber { get; set; } = string.Empty;

        [Required]
        [Range(1, 100)]
        public int MaxDistanceKm { get; set; }

        [Required]
        [Range(1, 7)]
        public int AvailableDaysPerWeek { get; set; }
    }