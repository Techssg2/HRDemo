using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UpdateUserInfoFromSapJobV2.src.Helpers
{
    public static class Utilities
    {
        public static void GetLinkType(string type, out string linkType, out string bizName)
        {
            linkType = string.Empty;
            bizName = string.Empty;
            switch (type)
            {
                case "ShiftExchangeApplication":
                    linkType = "shift-exchange";
                    bizName = "Shift Exchange";
                    break;
                case "Acting":
                    linkType = "action";
                    bizName = "Acting";
                    break;
                case "ResignationApplication":
                    linkType = "resignationApplication";
                    bizName = "Resignation";
                    break;
                case "RequestToHire":
                    linkType = "requestToHire";
                    bizName = "Request To Hire";
                    break;
                case "PromoteAndTransfer":
                    linkType = "promoteAndTransfer";
                    bizName = "Promotion and Transfer";
                    break;
                case "OvertimeApplication":
                    linkType = "overtimeApplication";
                    bizName = "Overtime";
                    break;
                case "MissingTimeClock":
                    linkType = "missingTimelock";
                    bizName = "Missing Time Lock";
                    break;
                case "LeaveApplication":
                    linkType = "leaves-management";
                    bizName = "Leave";
                    break;
                case "BusinessTripApplication":
                    linkType = "businessTripApplication";
                    bizName = "Business Trip Application";
                    break;
                case "TargetPlan":
                    linkType = "targetPlan";
                    bizName = "Target Plan";
                    break;
                // edoc 1
                case "Contract":
                    linkType = "Contract";
                    bizName = "Contract";
                    break;
                case "NonExpenseContract":
                    linkType = "Contract";
                    bizName = "Non-Expense Contract";
                    break;
                case "Payment":
                    linkType = "Payment";
                    bizName = "Payment";
                    break;
                case "Advance":
                    linkType = "Advance";
                    bizName = "Advance";
                    break;
                case "Reimbursement":
                    linkType = "Reimbursement";
                    bizName = "Business Trip Reimbursement";
                    break;
                case "ReimbursementPayment":
                    linkType = "ReimbursementPayment";
                    bizName = "Reimbursement Payment ";
                    break;
                case "Purchase":
                    linkType = "Purchase";
                    bizName = "Purchase ";
                    break;
                case "CreditNote":
                    linkType = "CreditNote";
                    bizName = "Credit Note";
                    break;
                case "TransferCash":
                    linkType = "TransferCash";
                    bizName = "Transfer Cash";
                    break;
                case "RefundCard":
                    linkType = "RefundCard";
                    bizName = "Refund Card";
                    break;
            }
        }
        public static void GetLinkTypeVN(string type, out string linkType, out string bizNameVN)
        {
            linkType = string.Empty;
            bizNameVN = string.Empty;
            switch (type)
            {
                case "ShiftExchangeApplication":
                    linkType = "shift-exchange";
                    bizNameVN = "Đơn đăng ký Chuyển ca";
                    break;
                case "Acting":
                    linkType = "action";
                    bizNameVN = "Đề xuất Tạm quyền";
                    break;
                case "ResignationApplication":
                    linkType = "resignationApplication";
                    bizNameVN = "Đơn đăng ký Nghỉ việc";
                    break;
                case "RequestToHire":
                    linkType = "requestToHire";
                    bizNameVN = "Đề xuất Tuyển dụng";
                    break;
                case "PromoteAndTransfer":
                    linkType = "promoteAndTransfer";
                    bizNameVN = "Đề xuất Thăng chức và thuyên chuyển";
                    break;
                case "OvertimeApplication":
                    linkType = "overtimeApplication";
                    bizNameVN = "Đơn đăng ký tăng ca";
                    break;
                case "MissingTimeClock":
                    linkType = "missingTimelock";
                    bizNameVN = "Phiếu bổ sung dữ liệu quyét thẻ";
                    break;
                case "LeaveApplication":
                    linkType = "leaves-management";
                    bizNameVN = "Đơn đăng ký Nghỉ phép";
                    break;
                case "BusinessTripApplication":
                    linkType = "businessTripApplication";
                    bizNameVN = "Đơn đăng kí công tác";
                    break;
                // edoc 1
                case "Contract":
                    linkType = "Contract";
                    bizNameVN = "Hợp đồng";
                    break;
                case "NonExpenseContract":
                    linkType = "NonExpenseContract";
                    bizNameVN = "Hợp đồng không tính phí";
                    break;
                case "Payment":
                    linkType = "Payment";
                    bizNameVN = "Thanh toán";
                    break;
                case "Advance":
                    linkType = "Advance";
                    bizNameVN = "Tạm ứng";
                    break;
                case "Reimbursement":
                    linkType = "Reimbursement";
                    bizNameVN = "Hoàn phí công tác";
                    break;
                case "ReimbursementPayment":
                    linkType = "ReimbursementPayment";
                    bizNameVN = "Thanh toán hoàn phí";
                    break;
                case "Purchase":
                    linkType = "Purchase";
                    bizNameVN = "Đơn hàng ";
                    break;
                case "CreditNote":
                    linkType = "CreditNote";
                    bizNameVN = "Tín dụng";
                    break;
                case "TransferCash":
                    linkType = "TransferCash";
                    bizNameVN = "Hoàn trả giao dịch thẻ";
                    break;
                case "RefundCard":
                    linkType = "RefundCard";
                    bizNameVN = "Rút tiền mặt chuyển tiền nội bộ";
                    break;
            }
        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
        public static StringContent StringContentObjectFromJson(string jsonContent)
        {
            StringContent result = new StringContent(jsonContent, UnicodeEncoding.UTF8, "application/json");
            return result;
        }
        public static string[] PendingStatuses()
        {
            return new string[] { "Completed", "Rejected", "Cancelled", "Draft" };
        }

        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }
        public static void UpdateMergeField(Dictionary<string, string> mergeFields, IEnumerable<string> bizTypes, bool edoc1)
        {
            if (bizTypes.Any())
            {
                var bizContent = "<ul>";
                var bizContentVN = "<ul>";
                foreach (var bizType in bizTypes)
                {
                    string linkType, bizName, bizNameVN;
                    Utilities.GetLinkType(bizType, out linkType, out bizName);
                    Utilities.GetLinkTypeVN(bizType, out linkType, out bizNameVN);
                    if (!string.IsNullOrEmpty(bizName))
                    {
                        bizContent += $"<li>{bizName}</li>";
                    }
                    if (!string.IsNullOrEmpty(bizNameVN))
                    {
                        bizContentVN += $"<li>{bizNameVN}</li>";
                    }
                }
                bizContent += "</ul>";
                bizContentVN += "</ul>";
                if (!edoc1)
                {
                    mergeFields["Edoc2BusinessName"] = bizContent;
                    mergeFields["Edoc2BusinessNameVN"] = bizContentVN;
                    mergeFields["Edoc2Link"] = $"<a href=\"{Convert.ToString(ConfigurationManager.AppSettings["siteUrl"])}/_layouts/15/AeonHR/Default.aspx#!/home/todo\">Link</a>";
                }
                else
                {
                    mergeFields["Edoc1BusinessName"] = bizContent;
                    mergeFields["Edoc1BusinessNameVN"] = bizContentVN;
                    mergeFields["Edoc1Link"] = $"<a href=\"{Convert.ToString(ConfigurationManager.AppSettings["edoc1Url"])}/default.aspx#/todo\">Link</a>";
                }

            }
            else
            {
                if (edoc1)
                {
                    mergeFields["Edoc1BusinessName"] = "------------------------------------------";
                    mergeFields["Edoc1BusinessNameVN"] = "------------------------------------------";
                    mergeFields["Edoc1Link"] = "";

                }
                else
                {
                    mergeFields["Edoc2BusinessName"] = "------------------------------------------";
                    mergeFields["Edoc2BusinessNameVN"] = "------------------------------------------";
                    mergeFields["Edoc2Link"] = "";
                }
            }
        }
        public static T Trim<T>(this T item)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in properties)
            {
                if (p.PropertyType != typeof(string) || !p.CanWrite || !p.CanRead) { continue; }
                var value = p.GetValue(item);
                if (value != null)
                {
                    p.SetValue(item, (value as string).Trim());
                }

            }
            return item;
        }
        public static bool IsValidTimeFormat(string input)
        {
            TimeSpan dummyOutput;

            return TimeSpan.TryParse(input, out dummyOutput);
        }

        public static DateTimeOffset ConvertStringToDatetime(string input)
        {
            if (string.IsNullOrEmpty(input))
                return DateTimeOffset.Now;
            //if (input.Split(' ').Length > 0)
            input = input.Trim();
            DateTimeOffset dummyOutput;
            if (DateTimeOffset.TryParseExact(input, "dd/MM/yyyy",
               CultureInfo.InvariantCulture,
               DateTimeStyles.None,
               out dummyOutput)
               ||
               (DateTimeOffset.TryParseExact(input, "d/M/yyyy",
               CultureInfo.InvariantCulture,
               DateTimeStyles.None,
               out dummyOutput))
               ||
               (DateTimeOffset.TryParseExact(input, "dd/M/yyyy",
               CultureInfo.InvariantCulture,
               DateTimeStyles.None,
               out dummyOutput))
               ||
               (DateTimeOffset.TryParseExact(input, "d/MM/yyyy",
               CultureInfo.InvariantCulture,
               DateTimeStyles.None,
               out dummyOutput))
               ||
               DateTimeOffset.TryParseExact(input, "dd-MM-yyyy",
               CultureInfo.InvariantCulture,
               DateTimeStyles.None,
               out dummyOutput)
               ||
               (DateTimeOffset.TryParseExact(input, "d-M-yyyy",
               CultureInfo.InvariantCulture,
               DateTimeStyles.None,
               out dummyOutput))
               ||
               (DateTimeOffset.TryParseExact(input, "dd-M-yyyy",
               CultureInfo.InvariantCulture,
               DateTimeStyles.None,
               out dummyOutput))
               ||
               (DateTimeOffset.TryParseExact(input, "d-MM-yyyy",
               CultureInfo.InvariantCulture,
               DateTimeStyles.None,
               out dummyOutput)))
            {
                return dummyOutput;
            }
            else
            {
                return DateTimeOffset.Now;
            }

        }
        public static bool IsValidDatetimeFormat(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            //if (input.Split(' ').Length > 0)
            //    input = input.Split(' ')[0];
            input = input.Trim();
            DateTimeOffset dummyOutput;
            if (DateTimeOffset.TryParseExact(input, "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dummyOutput)
                ||
                (DateTimeOffset.TryParseExact(input, "d/M/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dummyOutput))
                ||
                (DateTimeOffset.TryParseExact(input, "dd/M/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dummyOutput))
                ||
                (DateTimeOffset.TryParseExact(input, "d/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dummyOutput))
                ||
                DateTimeOffset.TryParseExact(input, "dd-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dummyOutput)
                ||
                (DateTimeOffset.TryParseExact(input, "d-M-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dummyOutput))
                ||
                (DateTimeOffset.TryParseExact(input, "dd-M-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dummyOutput))
                ||
                (DateTimeOffset.TryParseExact(input, "d-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dummyOutput)))
            {
                return true;
            }
            else
            {


                return false;
            }


        }

    }
}
