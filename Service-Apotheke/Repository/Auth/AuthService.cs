using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using ServiceApothekeAPI.Data;
using Service_Apotheke.Models;

using Service_Apotheke.Services.Email;
using Microsoft.EntityFrameworkCore;


namespace Service_Apotheke.Repository.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(AppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<PharmacistLoginResponseDto> RegisterPharmacist(PharmacistRegDto dto)
        {
            var existingPharmacist = await _context.Pharmacists
                .FirstOrDefaultAsync(p => p.Email == dto.Email);
            if (existingPharmacist != null)
                throw new Exception("Email already registered");

            var pharmacist = new Models.Pharmacist
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Phone = dto.Phone,
                Address = dto.Address,
                ConfirmationCode = GenerateConfirmationCode(),
                CreatedAt = DateTime.UtcNow
            };

            await _context.Pharmacists.AddAsync(pharmacist);
            await _context.SaveChangesAsync();

            await _emailService.SendConfirmationEmail(dto.Email, pharmacist.ConfirmationCode);

            // هنا بنولد التوكين فوراً
            var token = GenerateJwtToken(pharmacist.Id.ToString(), "Pharmacist", pharmacist.Email);

            return new PharmacistLoginResponseDto
            {
                Id = pharmacist.Id,
                FullName = pharmacist.FullName,
                Email = pharmacist.Email,
                Phone = pharmacist.Phone,
                Token = token,
                IsProfileCompleted = pharmacist.IsProfileCompleted,
                IsEmailConfirmed = pharmacist.IsEmailConfirmed
            };
        }



        public async Task<string> VerifyPharmacistEmail(VerifyDto dto)
        {
            var pharmacist = await _context.Pharmacists
                .FirstOrDefaultAsync(p => p.Email == dto.Email);

            if (pharmacist == null)
                return "Pharmacist not found";

            if (pharmacist.ConfirmationCode != dto.Code)
                return "Invalid confirmation code";

            pharmacist.IsEmailConfirmed = true;
            pharmacist.ConfirmationCode = string.Empty;

            await _context.SaveChangesAsync();

            return "Email verified successfully";
        }

        public async Task<PharmacistLoginResponseDto> LoginPharmacist(LoginDto dto)
        {
            var pharmacist = await _context.Pharmacists
                .FirstOrDefaultAsync(p => p.Email == dto.Email);

            if (pharmacist == null || !VerifyPassword(dto.Password, pharmacist.PasswordHash))
                return null;

            if (!pharmacist.IsEmailConfirmed)
                throw new Exception("Please verify your email first");

            var token = GenerateJwtToken(pharmacist.Id.ToString(), "Pharmacist", pharmacist.Email);

            return new PharmacistLoginResponseDto
            {
                Id = pharmacist.Id,
                FullName = pharmacist.FullName,
                Email = pharmacist.Email,
                Phone = pharmacist.Phone,
                Token = token,
                IsProfileCompleted = pharmacist.IsProfileCompleted,
                IsEmailConfirmed = pharmacist.IsEmailConfirmed
            };

        }

        public async Task<PharmacyLoginResponseDto> RegisterPharmacy(PharmacyRegDto dto)
        {
            var existingPharmacy = await _context.Pharmacies
                .FirstOrDefaultAsync(p => p.Email == dto.Email);
            if (existingPharmacy != null)
                throw new Exception("Email already registered");

            var pharmacy = new Models.Pharmacy
            {
                PharmacyName = dto.PharmacyName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Phone = dto.Phone,
                Address = dto.Address,
                LicenseNumber = dto.LicenseNumber,
                ConfirmationCode = GenerateConfirmationCode(),
                CreatedAt = DateTime.UtcNow
            };

            await _context.Pharmacies.AddAsync(pharmacy);
            await _context.SaveChangesAsync();

            await _emailService.SendConfirmationEmail(dto.Email, pharmacy.ConfirmationCode);

            var token = GenerateJwtToken(pharmacy.Id.ToString(), "Pharmacy", pharmacy.Email);

            return new PharmacyLoginResponseDto
            {
                Id = pharmacy.Id,
                PharmacyName = pharmacy.PharmacyName,
                Email = pharmacy.Email,
                Phone = pharmacy.Phone,
                Token = token,
                IsVerified = pharmacy.IsVerified
            };
        }
        public async Task<string> ForgetPharmacistPassword(string email)
        {
            var pharmacist = await _context.Pharmacists
                .FirstOrDefaultAsync(p => p.Email == email);

            if (pharmacist == null)
                throw new Exception("Email not found");

            pharmacist.ConfirmationCode = GenerateConfirmationCode();
            await _context.SaveChangesAsync();

            await _emailService.SendConfirmationEmail(email, pharmacist.ConfirmationCode);

            return "Reset password code sent to email";
        }
        public async Task<string> ResetPharmacistPassword(ResetPasswordDto dto)
        {
            var pharmacist = await _context.Pharmacists
                .FirstOrDefaultAsync(p => p.Email == dto.Email);

            if (pharmacist == null)
                throw new Exception("Pharmacist not found");

            if (pharmacist.ConfirmationCode != dto.Code)
                throw new Exception("Invalid reset code");

            pharmacist.PasswordHash = HashPassword(dto.NewPassword);
            pharmacist.ConfirmationCode = string.Empty;

            await _context.SaveChangesAsync();

            return "Password reset successfully";
        }
        public async Task<string> ForgetPharmacyPassword(string email)
        {
            var pharmacy = await _context.Pharmacies
                .FirstOrDefaultAsync(p => p.Email == email);

            if (pharmacy == null)
                throw new Exception("Email not found");

            pharmacy.ConfirmationCode = GenerateConfirmationCode();
            await _context.SaveChangesAsync();

            await _emailService.SendConfirmationEmail(email, pharmacy.ConfirmationCode);

            return "Reset password code sent to email";
        }
        public async Task<string> ResetPharmacyPassword(ResetPasswordDto dto)
        {
            var pharmacy = await _context.Pharmacies
                .FirstOrDefaultAsync(p => p.Email == dto.Email);

            if (pharmacy == null)
                throw new Exception("Pharmacy not found");

            if (pharmacy.ConfirmationCode != dto.Code)
                throw new Exception("Invalid reset code");

            pharmacy.PasswordHash = HashPassword(dto.NewPassword);
            pharmacy.ConfirmationCode = string.Empty;

            await _context.SaveChangesAsync();

            return "Password reset successfully";
        }


        public async Task<string> VerifyPharmacyEmail(VerifyDto dto)
        {
            var pharmacy = await _context.Pharmacies
                .FirstOrDefaultAsync(p => p.Email == dto.Email);

            if (pharmacy == null)
                return "Pharmacy not found";

            if (pharmacy.ConfirmationCode != dto.Code)
                return "Invalid confirmation code";

            pharmacy.IsVerified = true;
            pharmacy.ConfirmationCode = string.Empty;

            await _context.SaveChangesAsync();

            return "Email verified successfully";
        }

        public async Task<PharmacyLoginResponseDto> LoginPharmacy(LoginDto dto)
        {
            var pharmacy = await _context.Pharmacies
                .FirstOrDefaultAsync(p => p.Email == dto.Email);

            if (pharmacy == null || !VerifyPassword(dto.Password, pharmacy.PasswordHash))
                return null;

            if (!pharmacy.IsVerified)
                throw new Exception("Please verify your email first");

            var token = GenerateJwtToken(pharmacy.Id.ToString(), "Pharmacy", pharmacy.Email);

            return new PharmacyLoginResponseDto
            {
                Id = pharmacy.Id,
                PharmacyName = pharmacy.PharmacyName,
                Email = pharmacy.Email,
                Phone = pharmacy.Phone,
                Token = token,
                IsVerified = pharmacy.IsVerified
            };
        }

        public string GenerateJwtToken(string userId, string userType, string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim("UserType", userType),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            return hash == hashedPassword;
        }

        private string GenerateConfirmationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
