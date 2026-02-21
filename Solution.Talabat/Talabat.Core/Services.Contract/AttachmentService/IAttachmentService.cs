using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Services.Contract.AttachmentService
{
    public interface IAttachmentService
    {
        Task<string> UploadAsync(IFormFile file, string folderName); // Upload UrlImage

        bool Delete(string filePath);
    }
}
