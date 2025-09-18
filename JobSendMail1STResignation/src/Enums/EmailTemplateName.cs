using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMail1STResignation.src.Enums
{
    public enum EmailTemplateName
    {
        /// <summary>
        /// Cho người duyệt
        /// </summary>
        ForApprover = 1,
        /// <summary>
        /// Mail cho người tạo phiếu sau khi quy trình duyệt hoàn tất
        /// </summary>
        ForCreatorApproved = 2,
        /// <summary>
        /// Mail cho người tạo phiếu sau khi quy trình duyệt hoàn tất
        /// </summary>
        ForCreatorRejected = 3,
        /// <summary>
        /// Mail cho người tạo phiếu sau khi phiếu duyệt bị yêu cầu thay đổi thông tin (Request to change
        /// </summary>
        ForCreatorRequestToChange = 4,
        /// <summary>
        /// Mail cho người tạo phiếu khi push OT qua SAP bi loi
        /// </summary>
        CanNotPushOTRecord = 5,
        /// <summary>
        /// Layout for email
        /// </summary>
        MainLayout = 6,
        NewMSAccount = 7,
        NewADAccount = 8,
        ResetPassword = 9,
        /// <summary>
        /// Cho người được Assign To trong phiếu RTH sau khi phiếu được completed.
        /// </summary>
        ForAssigneeCompleted = 10,
        BTASendFileWhenComplete = 11,
        BTASendFileWhenCompleteChanging = 13,
        BTASendEmailWhenNextStepAdminChecker = 14,
        //lamnl
        BTASendEmailWhenDeleteBTADetails = 15,
        For1STResignation = 16
    }
}
