using Microsoft.AspNetCore.Http;
using Talabat.Core.Services.Contract.AttachmentService;

namespace Talabat.Services.AttachmentServices
{
    public class AttachmentService : IAttachmentService
    {
        // Extentions Allows
        private readonly List<string> _allowExtentions = new() { ".png", ".jpg", "jpeg" };
        private const int _allowMaxSize = 2_097_152;
        // Allow MaxSize

        public async Task<string> UploadAsync(IFormFile file, string folderName)
        {
            // get filePath
            var extention = Path.GetExtension(file.FileName);
            // check if has extention
            if (!_allowExtentions.Contains(extention))
                throw new Exception("Invaild File Extension are \".jpg\", \".png\", \".jpeg");
            // check on file size
            if(extention.Length > _allowMaxSize)
                throw new Exception("Invaild File Size, Max Allowed Size 2 Mbs");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath); // بتشك لو الفولدر ده مش موجود بروح اعمله الاول
            // 2. Create file name [Uniqu]
            var fileName = $"{Guid.NewGuid()}{extention}";
            // 3. Get file Path => file location based 
            var filePath = Path.Combine(folderPath, fileName); // file path location [URL]
            // 4. do streaming in filePath => علشان اجيب الداتا في وقتها
            using var fileStream = new FileStream(filePath, FileMode.Create); // open connection with database and close
            await file.CopyToAsync(fileStream); // take cope from file and put in fileStream

            return fileName; // علشان مكررش نفس مسار الفولدر كل مره
        }

        public bool Delete(string filePath)
        {
            if (File.Exists(filePath)) // Check if this file exist or no
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
    }
}
