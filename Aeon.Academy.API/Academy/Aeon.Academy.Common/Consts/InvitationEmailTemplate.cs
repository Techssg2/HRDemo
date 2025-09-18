using System.Text;

namespace Aeon.Academy.Common.Consts
{
    public class InvitationEmailTemplate
    {
        public const string BodyEmail = "<p>English below<br>---<br>Chào <b>[Employee Name]</b>,<br>Academy HQ xin kính mời các anh chị tham dự khóa học <b>[Course Name]</b> tổ chức bởi <b>[Supplier Name]</b><br>Thông tin khóa học:<br>Ngày giờ: <b>[Course Start Date]</b><br>Địa điểm: <b>[Location]</b><br>Chi tiết tham khảo xem file đính kèm<br>Anh chị vui lòng Accept thông báo mời học này trên hệ thống eDOC để xác nhận tham dự khóa học<br>Sau khóa học, anh chị vui lòng gửi các báo cáo sau:<br>Bản scan bằng hoàn tất khóa học (nếu có)<br>Deadline gửi report: <b>[After Training Report Deadline]</b><br><br>Cảm ơn các anh chị.<br>---<br>Dear <b>[Employee Name]</b>,<br>Academy HQ invited you to <b>[Course Name]</b> course organized by <b>[Supplier Name]</b><br>Training course information:<br>Date and time: <b>[Course Start Date]</b><br>Location: <b>[Location]</b><br>For further details please check the attachment in eDOC<br>Please help to confirm you will be joining the training course in eDOC<br>After training, please help to complete the External Supplier report<br>Including completed training certificate or confirmation (if available)<br>Please done your after-training report before <b>[After Training Report Deadline]</b><br><br><b>Best regards,</b><br>Academy department<br></p>";
        public const string HODBodyEmail = "<p>English below<br>---<br>Chào <b>[Employee HOD Name]</b>,<br>Academy HQ thông báo khóa học <b>[Course Name]</b> tổ chức bởi <b>[Supplier Name]</b> có nhân viên cấp dưới của bạn tham dự<br>Thông tin khóa học:<br>Ngày giờ: <b>[Course Start Date]</b><br>Địa điểm: <b>[Location]</b><br><br>Cảm ơn các anh chị.<br>---<br>Dear <b>[Employee HOD Name]</b>,<br>Academy HQ notified that one of your employees invited to <b>[Course Name]</b> course organized by <b>[Supplier Name]</b><br>Training course information:<br>Date and time: <b>[Course Start Date]</b><br>Location: <b>[Location]</b><br><br><b>Best regards,</b><br>Academy department<br></p>";
        public const string EmployeeName = "[Employee Name]";
        public const string EmployeeHODName = "[Employee HOD Name]";
        public const string Coursename = "[Course Name]";
        public const string ServiceProvider = "[Supplier Name]";
        public const string StartDate = "[Course Start Date]";
        public const string EndDate = "[EndDate]";
        public const string TrainingLocation = "[Location]";
        public const string AfterTrainingReportDeadline = "[After Training Report Deadline]";
        public const string AeonVN = "AEON VN";
        public const string IgnoreSectionIfNotReportVN = "<br>Sau khóa học, anh chị vui lòng gửi các báo cáo sau:<br>Bản scan bằng hoàn tất khóa học (nếu có)<br>Deadline gửi report: [After Training Report Deadline]<br>";
        public const string IgnoreSectionIfNotReportEN = "<br>After training, please help to complete the External Supplier report<br>Including completed training certificate or confirmation (if available)<br>Please done your after-training report before [After Training Report Deadline]<br>";
    }

}