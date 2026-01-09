using Microsoft.EntityFrameworkCore;
using Service_Apotheke.Services.File;
using ServiceApothekeAPI.Data;

namespace Service_Apotheke.Repository.Pharmacist
{
    public class PharmacistService : IPharmacistService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public PharmacistService(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }
        public async Task<List<Shift>> GetAllShifts(Guid pharmacistId)
        {
            return await _context.Shifts
                .Include(s => s.JobPost)
                .ThenInclude(jp => jp.Pharmacy)
                .Where(s => s.PharmacistId == pharmacistId)
                .OrderByDescending(s => s.JobPost.ShiftDate)
                .ToListAsync();
        }
        public async Task<Models.Pharmacist> GetById(Guid id)
        {
            // بنجيب الصيدلي بالـ ID بتاعه
            return await _context.Pharmacists.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<List<Shift>> GetUpcomingShifts(Guid pharmacistId)
        {
            return await _context.Shifts
                .Include(s => s.JobPost)
                .ThenInclude(jp => jp.Pharmacy)
                .Where(s => s.PharmacistId == pharmacistId
                       && s.Status == "Scheduled" // الحالة لسة مجدولة
                       && s.JobPost.ShiftDate >= DateTime.UtcNow.Date) // تاريخها النهاردة أو قدام
                .OrderBy(s => s.JobPost.ShiftDate)
                .ToListAsync();
        }

        public async Task<List<Shift>> GetCompletedShifts(Guid pharmacistId)
        {
            return await _context.Shifts
                .Include(s => s.JobPost)
                .ThenInclude(jp => jp.Pharmacy)
                .Where(s => s.PharmacistId == pharmacistId
                       && (s.Status == "Completed" || s.JobPost.ShiftDate < DateTime.UtcNow.Date))
                .OrderByDescending(s => s.JobPost.ShiftDate)
                .ToListAsync();
        }
        public async Task<bool> CompleteShift(Guid shiftId, CompleteShiftDto dto)
        {
            var shift = await _context.Shifts.FindAsync(shiftId);

            if (shift == null) return false;

            shift.Status = "Completed";
            shift.CheckOutTime = DateTime.UtcNow; // وقت ما داس انتهاء
            shift.PharmacistNotes = dto.PharmacistNotes; // الديسكريبشن اللي قولت عليه

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Notification>> GetPharmacistNotifications(Guid pharmacistId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == pharmacistId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        public async Task<bool> UpdateProfile(Guid id, UpdatePharmacistDto dto)
        {
            var pharmacist = await _context.Pharmacists.FindAsync(id);
            if (pharmacist == null)
                return false;

            pharmacist.FullName = dto.FullName;
            pharmacist.Phone = dto.Phone;
            pharmacist.Address = dto.Address;
            pharmacist.Country = dto.Country;
            pharmacist.Bio = dto.Bio;
            pharmacist.HasTransportation = dto.HasTransportation;
            pharmacist.TransportationType = dto.TransportationType;
            pharmacist.HasDrivingLicense = dto.HasDrivingLicense;
            pharmacist.TaxNumber = dto.TaxNumber;
            pharmacist.MaxDistanceKm = dto.MaxDistanceKm;
            pharmacist.AvailableDaysPerWeek = dto.AvailableDaysPerWeek;
            pharmacist.IsProfileCompleted = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> UploadCV(Guid id, IFormFile file)
        {
            var pharmacist = await _context.Pharmacists.FindAsync(id);
            if (pharmacist == null)
                throw new Exception("Pharmacist not found");

            var filePath = await _fileService.SaveCV(file);
            pharmacist.CvPath = filePath;

            await _context.SaveChangesAsync();
            return filePath;
        }
    }
}
