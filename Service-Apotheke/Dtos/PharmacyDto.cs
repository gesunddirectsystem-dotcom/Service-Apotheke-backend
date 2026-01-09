using System.ComponentModel.DataAnnotations;


    public class PharmacyRegDto
    {
        [Required]
        public string PharmacyName { get; set; } = string.Empty;

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

        [Required]
        public string LicenseNumber { get; set; } = string.Empty;
    }
    public class PharmacyLoginResponseDto
    {
        public Guid Id { get; set; }
        public string PharmacyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
    }
    public class UpdatePharmacyDto
    {
        [Required]
        public string PharmacyName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string LicenseNumber { get; set; } = string.Empty;
    }

