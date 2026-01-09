public interface IAuthService
{
    Task<PharmacistLoginResponseDto> RegisterPharmacist(PharmacistRegDto dto);
    Task<string> VerifyPharmacistEmail(VerifyDto dto);
    Task<PharmacistLoginResponseDto> LoginPharmacist(LoginDto dto);

    Task<PharmacyLoginResponseDto> RegisterPharmacy(PharmacyRegDto dto);
    Task<string> VerifyPharmacyEmail(VerifyDto dto);
    Task<PharmacyLoginResponseDto> LoginPharmacy(LoginDto dto);

    string GenerateJwtToken(string userId, string userType, string email);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
