using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Web;

namespace Aeon.HR.Infrastructure.Utilities
{
    public class UtilitiesHelper
    {
        public static bool CheckFormatFormula(String formula, Regex regrex)
        {
            var result = false; // Incorrect
            if (regrex != null)
            {
                var match = regrex.Match(formula);
                return match.Success;
            }
            return result;
        }
        public static byte[] ConvertToByteArrayFromBase64 (string base64Content)
        {
            if (string.IsNullOrEmpty(base64Content))
            {
                throw new ArgumentNullException(nameof(base64Content));
            }
            int indexOfSemiColon = base64Content.IndexOf(";", StringComparison.OrdinalIgnoreCase);
            string dataLabel = base64Content.Substring(0, indexOfSemiColon);
            string contentType = dataLabel.Split(':').Last();
            var startIndex = base64Content.IndexOf("base64,", StringComparison.OrdinalIgnoreCase) + 7;
            var fileContents = base64Content.Substring(startIndex);
            return Convert.FromBase64String(fileContents);
        }
    }
}