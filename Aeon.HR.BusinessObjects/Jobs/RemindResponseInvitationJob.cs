using Aeon.HR.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Jobs
{
    public class RemindResponseInvitationJob
    {
        private static readonly ILogger logger = ServiceLocator.Resolve<ILogger>();

        public async Task SendInvitation()
        {
            try
            {
                var moduleInstances = GetJobModules();
                if (moduleInstances != null || moduleInstances.Count() != 0)
                {
                    foreach (var mi in moduleInstances)
                    {
                        await mi.SendInvitation();
                    }
                }
            }
            catch (Exception error)
            {
                logger.LogError(error, "Error loading jobs from sub modules.");
            }
        }
        private static IEnumerable<Infrastructure.Interfaces.IRemindResponseInvitationJob> GetJobModules()
        {
            var instances = ServiceLocator.ResolveAll<Infrastructure.Interfaces.IRemindResponseInvitationJob>();
            return instances;
        }   
    }
}
