using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.DTOs
{
    public class FileResultDto
    {
        public string FileName { get; set; } // tên file muốn save xuống // có kèm đuôi luôn nha
        public object Content { get; set; } // byte array // base64, cái gì cũng được, miễn content của file trả về
        public string Type { get; set; } // MimeType
    }
}
