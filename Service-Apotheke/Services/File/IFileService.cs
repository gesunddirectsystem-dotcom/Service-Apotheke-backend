namespace Service_Apotheke.Services.File
{
    public interface IFileService
    {
        Task<string> SaveCV(IFormFile file);
    }
}
