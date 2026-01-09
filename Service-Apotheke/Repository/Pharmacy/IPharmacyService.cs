namespace Service_Apotheke.Repository.Pharmacy
{
    public interface IPharmacyService
    {
        Task<object> GetPharmacyFullDetails(Guid pharmacyId);
        Task<bool> UpdatePharmacy(Guid id, UpdatePharmacyDto dto);
    }
}
