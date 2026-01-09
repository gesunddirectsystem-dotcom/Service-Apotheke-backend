namespace Service_Apotheke.Services.File
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveCV(IFormFile file)
        {
            if (file == null) return null;

            // هنا بنحل مشكلة الـ Null بتاع Path1
            // لو الـ WebRootPath بـ null (ساعات بتحصل في بعض الاستضافات)، بنستخدم المسار الحالي
            string rootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // اسم الفولدر اللي هيتشال فيه الـ CV
            string uploadsFolder = Path.Combine(rootPath, "uploads", "cvs");

            // تأكد إن الفولدر موجود، ولو مش موجود اعمله
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // توليد اسم فريد للملف عشان ميتكررش
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // حفظ الملف فعلياً
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // بنرجع المسار النسبي عشان يتحفظ في الداتا بيز
            return Path.Combine("uploads", "cvs", uniqueFileName);
        }
    }
}
