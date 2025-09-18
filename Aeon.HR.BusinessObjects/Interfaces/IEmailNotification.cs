using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Interfaces
{
    
    public interface IEmailNotification
    {
        Task<bool> SendEmail(EmailTemplateName template, EmailTemplateName layoutName, Dictionary<string, string> mergedFields, List<string> recipients, Dictionary<string, byte[]> attachments = null, List<string> ccRecipients = null);
    }
}
