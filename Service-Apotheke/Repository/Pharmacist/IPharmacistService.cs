namespace Service_Apotheke.Repository.Pharmacist
{
    public interface IPharmacistService
    {
        Task<bool> CompleteShift(Guid shiftId, CompleteShiftDto dto);
        Task<Models.Pharmacist> GetById(Guid id);
        Task<bool> UpdateProfile(Guid id, UpdatePharmacistDto dto);
        Task<List<Shift>> GetAllShifts(Guid pharmacistId);
        Task<List<Shift>> GetUpcomingShifts(Guid pharmacistId);
        Task<List<Shift>> GetCompletedShifts(Guid pharmacistId);
        Task<List<Notification>> GetPharmacistNotifications(Guid pharmacistId);
        Task<string> UploadCV(Guid id, IFormFile file);
    }

}
