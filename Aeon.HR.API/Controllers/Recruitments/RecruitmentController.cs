using Microsoft.Extensions.Logging;
using Aeon.HR.BusinessObjects.Interfaces;
using SSG2.API.Controllers.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SSG2.API.Controllers.Recruitments
{
    public class RecruitmentController : BaseController
    {
        protected readonly IRecruitmentBO _recruitmentBO;
        public RecruitmentController(ILogger logger, IRecruitmentBO recruitment) : base(logger)
        {
            _recruitmentBO = recruitment;
        }
    }
}
