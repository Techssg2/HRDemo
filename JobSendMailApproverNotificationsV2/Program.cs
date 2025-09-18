using JobSendMailApproverNotificationsV2.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSendMailApproverNotificationsV2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Utilities.CleanupOldLogs(30);
                Utilities.WriteLogError("//----------------------------------------JobSendMailApproverNotifications started-----------------------------------------//");
                Utilities.WriteLogError("\n\n");
                var job = new JobSendMailApproverNotifications();
                await job.Run();
                Utilities.WriteLogError("\n\n");
                Utilities.WriteLogError("//---------------------------------JobSendMailApproverNotifications completed successfully---------------------------------//");
            }
            catch (Exception ex)
            {
                Utilities.WriteLogError("//-----------------------------------------JobSendMailApproverNotifications failed-----------------------------------------//");
                Utilities.WriteLogError(ex);
            }
        }
    }
}
