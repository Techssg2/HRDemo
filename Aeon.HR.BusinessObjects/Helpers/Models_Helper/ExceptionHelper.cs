using Aeon.HR.BusinessObjects.DataHandlers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.Data;
using Newtonsoft.Json;
using System.Reflection;
using Aeon.HR.ViewModels;
using Aeon.HR.Infrastructure.Enums;
using Microsoft.Extensions.Logging;

namespace Aeon.HR.BusinessObjects
{
    #region ExceptionHelper
    public static class ExceptionHelper
    {
        public static void LogError(this Exception ex, ILogger logger, string methodName)
        {
            try
            {
                logger.LogError($"Error at {methodName}", ex.Message +". "+ex.StackTrace);
            }
            catch 
            {
            }
        }
    }
    #endregion
}
