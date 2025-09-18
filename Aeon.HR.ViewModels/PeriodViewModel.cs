using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class PeriodViewModel
    {
        public DateTimeOffset FromDate { get; set; } // thời gian bắt đầu của period
        public DateTimeOffset ToDate { get; set; } // thời gian kết thúc của period
        public string Appraising { get; set; } // một json lưu lại table nội dung trong group appraising của mỗi period. Ví dụ [{goal: 'Hoàn thành bộ chỉ tiêu đánh giá', weight: 'Free text', actual: 'free text'}] // weight là trọng số
        public Guid ActingId { get; set; } // dùng để xác đinh là period này thuộc về acting nào
        public Priority Priority { get; set; }
    }
}