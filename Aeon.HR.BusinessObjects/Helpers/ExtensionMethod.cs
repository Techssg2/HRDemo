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

namespace Aeon.HR.BusinessObjects.Helpers
{
    public static class ExtensionMethod
    {
        /// <summary>
        /// Convert Datetime thành yyyyMMdd cho SAP
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToSAPFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }
        public static string ToSAPFormatString(this string value)
        {
            if (!String.IsNullOrEmpty(value) && value.Length == 6 && value.Substring(0, 2) != "00")
            {
                return String.Format("00{0}", value);
            }
            return value;
        }
        public static DateTime ToDateTime(this string value)
        {
            DateTime rs = new DateTime();
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    rs = DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture);
                }
            }
            catch { }
            return rs;
        }
        public static object GetPropValue(this object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static QueryArgs AddPredicate(this QueryArgs args, string nPredicate, object nPredicateParam)
        {
            try
            {
                var predicateParams = args.PredicateParameters.ToList();
                predicateParams.Add(nPredicateParam);
                args.PredicateParameters = predicateParams.ToArray();
                nPredicate = nPredicate.Replace("[index]", $"{args.PredicateParameters.Length - 1}");
                args.Predicate += (args.Predicate.Length > 0 ? " && " : string.Empty) + nPredicate;
                return args;
            }
            catch
            {
                return null;
            }
        }

        // Hàm validation dựa vào attribute của thuộc tính trong model
        public static ValidationResultDto TryValidateModel<T>(this T model)
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(model, context, validationResults, true);
            return new ValidationResultDto { IsValid = isValid, ValidationResults = validationResults };
        }

        public static void TryValidateModel<T>(this T model, ResultDTO resultDTO)
        {
            var validationResultDto = model.TryValidateModel();

            // chuyển message vào bên trong resultDTO
            if (validationResultDto.ValidationResults.Count != 0)
            {
                var errorMessages = validationResultDto.ValidationResults.Select(x => x.ErrorMessage);
                resultDTO.Messages.AddRange(errorMessages);
                resultDTO.ErrorCodes.Add(0); // mã error validation model
            }
        }
        public static async Task<HttpResponseMessage> PostData(this HttpClient client, string url, StringContent content)
        {
            return await client.PostAsync(url, content);
        }
    }
}