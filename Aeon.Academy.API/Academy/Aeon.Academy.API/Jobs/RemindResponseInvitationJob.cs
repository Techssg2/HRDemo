using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeon.Academy.Services;
using Aeon.HR.Infrastructure.Interfaces;

namespace Aeon.Academy.API.Jobs
{
    public class RemindResponseInvitationJob : IRemindResponseInvitationJob

    {
        private readonly IDoTaskService doTaskService;
        private readonly IUserService userService;

        public RemindResponseInvitationJob(IDoTaskService doTaskService, IUserService userService)
        {
            this.doTaskService = doTaskService;
            this.userService = userService;
        }
        public async Task SendInvitation()
        {
            await Task.Run(() => doTaskService.SendInviteeNotification());
        }
    }
}
