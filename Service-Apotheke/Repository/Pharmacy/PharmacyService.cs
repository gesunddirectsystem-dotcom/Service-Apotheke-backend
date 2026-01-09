using Microsoft.EntityFrameworkCore;
using ServiceApothekeAPI.Data;
namespace Service_Apotheke.Repository.Pharmacy
{
    public class PharmacyService : IPharmacyService
    {
        private readonly AppDbContext _context;

        public PharmacyService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<object> GetPharmacyFullDetails(Guid pharmacyId)
        {
            var pharmacy = await _context.Pharmacies
                .Include(p => p.JobPosts)
                    .ThenInclude(jp => jp.JobApplications)
                        .ThenInclude(ja => ja.Pharmacist) // عشان نشوف مين اللي قدم
                .FirstOrDefaultAsync(p => p.Id == pharmacyId);

            if (pharmacy == null) return null;

            return new
            {
                pharmacy.Id,
                pharmacy.PharmacyName,
                pharmacy.Email,
                pharmacy.Phone,
                pharmacy.Address,
                pharmacy.Country,
                pharmacy.LicenseNumber,
                pharmacy.IsVerified,
                Posts = pharmacy.JobPosts.Select(jp => new {
                    jp.Id,
                    jp.ShiftDate,
                    jp.Salary,
                    jp.Status,
                    ApplicationsCount = jp.JobApplications.Count,
                    Applicants = jp.JobApplications.Select(ja => new {
                        ja.Id,
                        ja.PharmacistId,
                        ja.Pharmacist.FullName,
                        ja.Pharmacist.Phone,
                        ja.Status,
                        ja.AppliedAt
                    })
                })
            };
        }
        public async Task<bool> UpdatePharmacy(Guid id, UpdatePharmacyDto dto)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(id);
            if (pharmacy == null)
                return false;

            pharmacy.PharmacyName = dto.PharmacyName;
            pharmacy.Phone = dto.Phone;
            pharmacy.Address = dto.Address;
            pharmacy.Country = dto.Country;
            pharmacy.LicenseNumber = dto.LicenseNumber;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
