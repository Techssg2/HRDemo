using Aeon.HR.BusinessObjects.DataHandlers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Dynamic;

namespace Aeon.HR.BusinessObjects
{
    #region DateTime Helper
    public static class SearchFlightHelper
    {
        public const string GOTADI_DATE_FORMAT = "MM-dd-yyyy";

        public static string GetAsGotadiDateFormat(this DateTime dateTime)
        {
            string returnValue = string.Empty;
            try
            {
                if(dateTime != DateTime.MinValue)
                {
                    returnValue = dateTime.ToString(GOTADI_DATE_FORMAT);
                }
            }
            catch
            {
                returnValue = string.Empty;
            }
            return returnValue;
        }

    }
    #endregion
}
