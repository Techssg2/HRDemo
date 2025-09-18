using Aeon.HR.API.Controllers.Others;
using Aeon.HR.BusinessObjects;
using Aeon.HR.BusinessObjects.CompleteActions;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.BusinessObjects.Jobs;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.CustomSection;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using PdfSharp.Pdf.Advanced;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using TargetPlanTesting.ImportData;

namespace SSG2.API.Controllers.Others
{
    public class SSGExController : ApiController
    {
        private readonly ISSGExBO _bo;
        private readonly IEmailNotification _email;
        private readonly IIntegrationExternalServiceBO _externalServiceBO;
        private readonly IRecruitmentBO _recruitment;
        protected readonly ILogger _logger;
        protected readonly IEmployeeBO _employee;
        protected readonly IUnitOfWork unitOf;
        private readonly IEdoc01BO _edoc01BO;
        private readonly IDashboardBO _dashboardBO;
        private readonly IWorkflowBO _workflowBO;
        private readonly ISSGExBO _exBO;
        private readonly ISettingBO _settingBO;
        private readonly IEmailNotification _emailNotificationBO;

        public SSGExController(
            ISSGExBO bo,
            ILogger logger,
            IEmailNotification email,
            IIntegrationExternalServiceBO externalServiceBO,
            IRecruitmentBO recruitment,
            IEmployeeBO employee,
            IUnitOfWork unitOfWork,
            IEdoc01BO edoc01,
            IDashboardBO dashboardBO,
            IWorkflowBO workflowBO,
            ISSGExBO exBO,
            ISettingBO settingBO,
            IEmailNotification emailNotificationBO
            )
        {
            _bo = bo;
            _externalServiceBO = externalServiceBO;
            _email = email;
            _recruitment = recruitment;
            _logger = logger;
            _employee = employee;
            unitOf = unitOfWork;
            _edoc01BO = edoc01;
            _dashboardBO = dashboardBO;
            _workflowBO = workflowBO;
            _exBO = exBO;
            _settingBO = settingBO;
            _emailNotificationBO = emailNotificationBO;
        }
        [HttpPost]
        public async Task<ResultDTO> UpdateEmployeeStatus(StatusArgs statusArgs)
        {
            try
            {
                var result = await _bo.UpdateEmployeeStatus(statusArgs);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateUser", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> TeminateEmployee(string SAPCode)
        {
            try
            {
                var result = await _bo.TeminateEmployee(SAPCode);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: CreateUser", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        #region Get SAP Data

        [HttpPost]
        public async Task<ResultDTO> GetLeaveBalanceSet(string sapCode)
        {
            var yearForTest = "2020";
            try
            {
                if (!string.IsNullOrEmpty(sapCode))
                {
                    return await _bo.GetLeaveBalanceSet(sapCode);

                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetLeaveBalanceSet", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetLeaveBalanceSet(string sapCode, int year)
        {
            var yearForTest = "2020";
            try
            {
                if (!string.IsNullOrEmpty(sapCode))
                {
                    return await _bo.GetLeaveBalanceSet(sapCode, year);

                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetLeaveBalanceSet(string sapCode, int year)", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetOvertimeBalanceSet(string sapCode, Guid? OvertimeApplicationid)
        {
            try
            {
                if (!string.IsNullOrEmpty(sapCode))
                {
                    return await _bo.GetOvertimeBalanceSet(sapCode, OvertimeApplicationid);
                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetLeaveBalanceSet(string sapCode, int year)", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetMultiOvertimeBalanceSet(List<OverTimeBalanceArgs> model)
        {
            try
            {
                if (model.Any())
                {
                    return await _bo.GetMultiOvertimeBalanceSet(model);
                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetLeaveBalanceSet(string sapCode, int year)", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetOTHourInMonth(List<OverTimeBalanceArgs> model)
        {
            try
            {
                if (model.Count > 0)
                {
                    return await _bo.GetOTHourInMonth(model);
                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetOTHourInMonth(string sapCode)", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetMultipleLeaveBalanceSet(List<string> sapCodes)
        {
            try
            {
                if (sapCodes.Count > 0)
                {
                    return await _bo.GetMultipleLeaveBalanceSet(sapCodes);
                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetMultipleLeaveBalanceSet", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> UpdateUserInformationQuoteFromSAP(CommonArgs.SAP.UpdateUserInformationQuoteFromSAP args)
        {
            try
            {
                if (args.SapCodes.Count > 0)
                {
                    return await _bo.UpdateUserInformationQuoteFromSAP(args);
                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: AsyncSAPCode", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetPromotionSet(string sapCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(sapCode))
                {
                    var predicate = "Pernr='@0'";
                    var predicateParameters = new string[] { sapCode };
                    var excution = _externalServiceBO.BuildAPIService(ExtertalType.PromoteAndTransfer);
                    excution.APIName = "Promote_TransferSet";
                    var res = await excution.GetData(predicate, predicateParameters);
                    return new ResultDTO { Object = res };
                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetPromotionSet", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetActingSet(string sapCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(sapCode))
                {
                    var predicate = "Pernr='@0'";
                    var predicateParameters = new string[] { sapCode };
                    var excution = _externalServiceBO.BuildAPIService(ExtertalType.Acting);
                    excution.APIName = "ActingSet";
                    var res = await excution.GetData(predicate, predicateParameters);
                    return new ResultDTO { Object = res };
                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: ActingSet", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetResignationDataSet(string sapCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(sapCode))
                {
                    var predicate = "Pernr='@0'";
                    var predicateParameters = new string[] { sapCode };
                    var excution = _externalServiceBO.BuildAPIService(ExtertalType.Resignation);
                    excution.APIName = "ResignationSet";
                    var res = await excution.GetData(predicate, predicateParameters);
                    return new ResultDTO { Object = res };
                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: GetResignationDataSet", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "No Connection" } };
            }
        }
        #endregion
        [HttpGet]
        public async Task<ResultDTO> TestMail()
        {
            try
            {
                var mergeField = new Dictionary<string, string>();
                mergeField.Add("BusinessName", "Overtime Application");
                mergeField.Add("CreatorName", "Đinh Trung Kiên");
                mergeField.Add("Link", "https://devgiangho.github.io");
                mergeField.Add("ApproverName", "Đinh Trung Kiênv");
                var recipients = new List<string>();
                recipients.Add("kiendt@carp.vn");

                var result = await _email.SendEmail(EmailTemplateName.ForApprover, EmailTemplateName.ForApprover, mergeField, recipients);
                return new ResultDTO { Object = result };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: TestMail", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> SubmitApplicant(Guid ItemId)
        {
            try
            {
                var res = await _recruitment.SubmitApplicant(ItemId);
                return new ResultDTO { Object = res };

            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: SubmitApplicant", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> Retry(Guid Id)
        {
            try
            {
                return await _bo.Retry(Id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at ReTry", ex.Message);
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }
        [HttpGet]
        public async Task<ResultDTO> Test(Guid Id, int type)
        {
            // type = 1: leave
            // type = 2: miss
            // type = 3: OT
            // type = 4: shift

            try
            {
                IIntegrationEntity data = null;
                if (type == 1)
                {
                    data = await unitOf.GetRepository<LeaveApplication>(true).FindByIdAsync(Id);
                }
                else if (type == 2)
                {
                    data = await unitOf.GetRepository<MissingTimeClock>(true).FindByIdAsync(Id);
                }
                else if (type == 3)
                {
                    data = await unitOf.GetRepository<OvertimeApplication>(true).FindByIdAsync(Id);
                }
                else if (type == 4)
                {
                    data = await unitOf.GetRepository<ShiftExchangeApplication>(true).FindByIdAsync(Id);
                }
                else if (type == 5)
                {
                    data = await unitOf.GetRepository<TargetPlan>(true).FindByIdAsync(Id);
                }
                await _externalServiceBO.SubmitData(data, true);
                var result = await unitOf.GetRepository<TrackingRequest>().FindByAsync<TrackingRequestForGetListViewModel>(x => x.ReferenceNumber == ((WorkflowEntity)data).ReferenceNumber);
                return new ResultDTO { Object = result };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at ReTry", ex.Message);
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> CreatedPayload(CreatePayloadArgs args)
        {
            try
            {
                if (args != null && args.ReferenceNumbers.Any())
                {
                    foreach (var refItem in args.ReferenceNumbers)
                    {
                        IIntegrationEntity data = null;
                        string refNumber = refItem.Substring(0, 3);
                        switch (refNumber)
                        {
                            case "LEA":
                                data = await unitOf.GetRepository<LeaveApplication>(true).GetSingleAsync(x => x.ReferenceNumber == refItem);
                                break;
                            case "MIS":
                                data = await unitOf.GetRepository<MissingTimeClock>(true).GetSingleAsync(x => x.ReferenceNumber == refItem);
                                break;
                            case "SHI":
                                data = await unitOf.GetRepository<ShiftExchangeApplication>(true).GetSingleAsync(x => x.ReferenceNumber == refItem);
                                break;
                            case "TAR":
                                data = await unitOf.GetRepository<TargetPlan>(true).GetSingleAsync(x => x.ReferenceNumber == refItem);
                                break;
                            case "OVE":
                                data = await unitOf.GetRepository<OvertimeApplication>(true).GetSingleAsync(x => x.ReferenceNumber == refItem);
                                break;
                            case "RES":
                                data = await unitOf.GetRepository<ResignationApplication>(true).GetSingleAsync(x => x.ReferenceNumber == refItem);
                                break;
                            case "PRO":
                                data = await unitOf.GetRepository<PromoteAndTransfer>(true).GetSingleAsync(x => x.ReferenceNumber == refItem);
                                break;
                            case "ACT":
                                data = await unitOf.GetRepository<Acting>(true).GetSingleAsync(x => x.ReferenceNumber == refItem);
                                break;
                        }
                        if (data != null)
                        {
                            await _externalServiceBO.SubmitData(data, false);
                        }
                    }
                }
                return new ResultDTO { Object = "Ok" };
            }
            catch (Exception ex)
            {
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> UpdatePayload(UpdatePayloadArgs args)
        {
            try
            {
                return await _bo.UpdatePayload(args);
            }
            catch (Exception ex)
            {
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }

        private class ModelDataCommit
        {
            public string Data { get; set; }
            public string Key { get; set; }
        }

        public string GetKeyToString(string publicKeyFilePath)
        {
            if (!publicKeyFilePath.StartsWith("\\"))
            {
                publicKeyFilePath = "\\" + publicKeyFilePath;
            }
            string filepath = HttpContext.Current.Server.MapPath(publicKeyFilePath);
            return File.ReadAllText(filepath);
        }
        public string ConvertPemToXml(string pem)
        {
            // Đọc khóa từ PEM bằng Bouncy Castle
            AsymmetricCipherKeyPair keyPair;
            // pem = pem.Replace("\r", "").Replace("\n", "\n");
            using (var reader = new StringReader(pem))
            {
                var pemReader = new PemReader(reader);
                keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
            }

            // Chuyển đổi sang định dạng .NET RSAParameters
            var rsaParams = DotNetUtilities.ToRSAParameters((Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)keyPair.Private);

            // Tạo XML chứa các giá trị RSA
            var xml = new XElement("RSAKeyValue",
                new XElement("Modulus", Convert.ToBase64String(rsaParams.Modulus)),
                new XElement("Exponent", Convert.ToBase64String(rsaParams.Exponent)),
                new XElement("D", Convert.ToBase64String(rsaParams.D)),
                new XElement("P", Convert.ToBase64String(rsaParams.P)),
                new XElement("Q", Convert.ToBase64String(rsaParams.Q)),
                new XElement("DP", Convert.ToBase64String(rsaParams.DP)),
                new XElement("DQ", Convert.ToBase64String(rsaParams.DQ)),
                new XElement("InverseQ", Convert.ToBase64String(rsaParams.InverseQ))
            );

            return xml.ToString();
        }

        public static byte[] DecryptRSAToByte(string encryptedKey, RSA privateKey)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedKey);
            return privateKey.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
        }

        public static RSA CreateCipherDecrypt(string privateKeyPem)
        {
            using (TextReader reader = new StringReader(privateKeyPem))
            {
                PemReader pemReader = new PemReader(reader);
                AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);

                RSA rsa = RSA.Create();
                rsa.ImportParameters(rsaParams);
                return rsa;
            }
        }

        public string ConvertBase64(string data)
        {
            string _return = data.Replace('-', '+').Replace('_', '/');

            // Thêm padding nếu thiếu
            switch (_return.Length % 4)
            {
                case 2: _return += "=="; break;
                case 3: _return += "="; break;
            }
            return _return;
        }


        [HttpPost]
        public async Task<ResultDTO> TestJob(ModelTestJob model)
        {
            try
            {
                GotadiSettingsSection GOTADI_CONFIG = (GotadiSettingsSection)ConfigurationManager.GetSection("airTicketSettings");
                string publicKeyGotadi = GetKeyToString(GOTADI_CONFIG.Certificates.Gotadi_Public_Key);
                // string publicKeyGotadi = RSAHelper.GetPublicKey(GOTADI_CONFIG.Certificates.Gotadi_Public_Key);
                string privateKeyGotadi = GetKeyToString(GOTADI_CONFIG.Certificates.AEON_BTA_Private_Key);
                //string privateKeyGotadi = RSAHelper.GetPrivateKey(GOTADI_CONFIG.Certificates.AEON_BTA_Private_Key);


                publicKeyGotadi = publicKeyGotadi.Replace("\r", "").Replace("\n", "\n");
                privateKeyGotadi = privateKeyGotadi.Replace("\r", "").Replace("\n", "\n");

                //RSACryptoServiceProvider gtdPublicKey = RSAHelper.GetCryptoServiceByPublicKey(Convert.FromBase64String(RSAHelper.GetPublicKey(GOTADI_CONFIG.Certificates.Gotadi_Public_Key)));
                //RSACryptoServiceProvider gtdPrivateKey = RSAHelper.GetCryptoServiceByPrivateKey(Convert.FromBase64String(RSAHelper.GetPrivateKey(GOTADI_CONFIG.Certificates.AEON_BTA_Private_Key)));

                string keyCommit = "ZO-l9YyW-svaSBOneyFHByZxqiMt4f0eDlflo5vfS8BoVbMjStm4qxhbFuow8JQz2L79YimyYBK9CIsobQX0wubNTI_6XDfpBKkJ7hBEGx7x2aIEcXjF6Twun_fHdJ44XXn1_9nen0DUdTpDnjs4NIQLMD-Z1mkwgijtYHpd8qk";
                string dataCommit = "3SRp74VaZRfQg4Yldx-KV2hVRbeUgmVtsRYq0MQy0TFMWv2ityPJ9l2JRcEwATaSJaU7yVii3CkcEqos8fm_Qw5C_uce43olJI6mK3oGzYzwpdwnpFf-Y1rGdZnDEVAm-A5obQCysKrIvd-aH9MbgrI5oXkOHr6Hii8q2USAgGWpnzrhf_yu0wuWJ6-aQWa6enyyz1jVBJ8uT48keZFf9SxvW68-fYUXkRiCd97m9RQO7ESjkfp6pudk-JfHzwonii8q2USAgGWpnzrhf_yu0wuWJ6-aQWa6VBVW8AyqyN-pfpZVIq0JI_9z583ehXGbW9g2lP12-fDroTAz_1KkqWUCjDNEn-LoOTliaxjDuB2Na7LEnBnEaZ0zuA2ZoZ_iGKedmn-aQoxtrg5nzZf56JtxtGf1XRfp1EeSlUC5TBz2_HbwdGlnpnPpl2J2sNjHiSMSJj3a4LCvABNRGAHCzpKcbkzyCjPgW6xbT5HkfCGBwyXmTe3za-qtiktQnW3dCCDokF-Gnl4-o93KEX7_MLFFFLwfcIysaJft8cwpz-s71Gxxlgm5ZnmyyGv9yvZo0YLQ91YFhfzTXbDVlqueeNyAxpyIVfzJFhGaxWrLZtbeXiPp5wdRTPoGpBHTO2Wcay1WtdF-s9elsziQbl1t7nGiNexAknSg0nXl3t3ZbMW2fTtUF3av3FOskMiaJqQzTTF8shRs5C0l8fvpuD-tI9PmK-cC1IFrtpGZ7sOYTmHapGEYGAgE2ytjpDO0hr1-R0csGkYS4vBuFS2xTE3Q_j0mEV1pSQqiTxD5HX8bgdsluw80qsqyUzPpTA7Qgd3Vv4VqUZtvPZtrCepkOr_RHP7F94-QFg6sRqkzKTVEaQX17v12cZyPBmFdB0GTriGBa1MeyrqJHC3q9OPSOic7AWLxJhlPV0lhnwQAMTHNV2htVMf_o5CLUunRiv51EsP6Q3c2kGfQ2sttyzIeHa6QWs1k-ENQs1Ir4SOeBWtQBX_CPRMdP6uaxdzszlc2QwzRzFSYO8TQ6WU";

                keyCommit = model.Key;
                dataCommit = model.Data;

                keyCommit = ConvertBase64(keyCommit);

                RSA privateKey = CreateCipherDecrypt(privateKeyGotadi);
                byte[] decryptedBytes = DecryptRSAToByte(keyCommit, privateKey);


                // byte[] randomKey = DecryptRSAToByte(keyCommit, privateKeyGotadi);

                dataCommit = ConvertBase64(dataCommit);
                string originalData = CryptoUtils.DecryptTripleDes(dataCommit, decryptedBytes);

                // Step 1: Decrypt random 3DES key.
                // byte[] randomKey = CryptoUtils.DecryptRSAToByte(keyCommit, privateKey);

                // Step 2: Encrypt data
                //String originalData = CryptoUtils.DecryptTripleDes(dataCommit, randomKey);

                /*string keyCommit = "kAJ5q47p_LtXF1ur6CQYQclYImPFaVAohcTZVfVEpIbTgpPwtBGmj8GB1sRbPQPaeetW2VUzkA0hqXwmDLAEsmRj83tB7AOk8SC56olTtbDi8EFKsZ2mGBU-oChtdiwChJB7zIEBx4pMu5dcUKojDMDBYUcC-ALLlfr3zbZJuO-gguqL3E4FPw7eXZ-jbaSYZK5EabdbVOqg0bJyzTqJbIUqyjMuc6yr2gP1M9rPWhQHmsZClLnGqM2Tvvmz2Tddlfr3zbZJuO-gguqL3E4FPw7eXZ-jbaSYh2-2gN5HblKs7aaHKd5INO9yJ4r9IUabSm3ko27dlAhGa06kg8eJGeuU0_nF5VCiwRNqNTJibU55gE0AN1lguyTzVxZJ2R77nerjeBuTVCLP_fVgaNe4UE0nNaF_5cTqe0c7GGf52oMuY9YOOnswpZqsT5xKE9ErrNQJf2VrwT5XwdSUiPCa-bpH725uow2HTvq5MZkGXZE4P2NaWqlRrQBaK9PmMzUWD5r1raKFDgN-TU_Jj0UC5SoYY5zrT5JRud9VQkHYKO5cAKAwjS1aTEVmZxyBdqcZTu5QgXMbZ01uDhO7UAyJQpo9iewMEubUaHeHowsdyaO0iv2O4zSJ9LDvyG4IpZSIN2Fezpt3PkL_bIWOWNg2t8DKyhFrpTa7FYsjAt1ol-GyfndbrLzbl0n85CqcPyujFvuEvmihpLymXIwDgSuAznrtLBRAK_FcCC73XM0LDcMuU4vWV3_3Hng9x4CW_ZmpMuaMDEF4hH3urRiVple9Dvu0TWzUR_tQxNNGmG-pq5IQ1a7zaWC2eXp1r75J6miWXeIn0BeBbFKTpYmBbcAwuFWDURCxnS5aiEqIGaKIbvZTdd7bPZoRsW1H6Q2J2zjUnNveKlNhceObNqw686G8jOlQ7cyztv3RTNr8Y8lG9aX0kuV6xYNrxfS87iB9hPdelcIFmr_BamKB76FJzzBflR8FEIwQKvRMUqwsdVsHbHY5Afpbyo4QEbZtdsHOtJe9";
                string dataCommit = "H3GiyHYt939bspX_34j5DoLOUjYg2oFQYrZ2UGInnMu-bUsj-6QHpeywS1znAYU26m1CKzm8pODgURvAfWNwXg2xMbQo5sKigxmg8orEK1aJhwg3jr0DRC5k7pxpqvFf4lOQN8lpBxGDE50bVMtuA3Q8YaQYBPYSDBZHvRQy1CI";

                this.gotadiPublicKey = CryptoUtils.getPublicKeyFromXML(System.getenv("GTD_PUBLIC_KEY"));
                this.privateKey = CryptoUtils.getPrivateKeyFromXML(System.getenv("PRIVATE_KEY"));*/



                /*PromoteAndTransferJob job = new PromoteAndTransferJob(_logger, unitOf, _settingBO);
                await job.Sync();*/
                return new ResultDTO { Object = originalData };
            }
            catch (TaskCanceledException ex)
            {
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> ConvertCommitBTA(ModelTestJob model)
        {
            try
            {
                GotadiSettingsSection GOTADI_CONFIG = (GotadiSettingsSection)ConfigurationManager.GetSection("airTicketSettings");
                string publicKeyGotadi = GetKeyToString(GOTADI_CONFIG.Certificates.Gotadi_Public_Key);
                // string publicKeyGotadi = RSAHelper.GetPublicKey(GOTADI_CONFIG.Certificates.Gotadi_Public_Key);
                string privateKeyGotadi = GetKeyToString(GOTADI_CONFIG.Certificates.AEON_BTA_Private_Key);
                //string privateKeyGotadi = RSAHelper.GetPrivateKey(GOTADI_CONFIG.Certificates.AEON_BTA_Private_Key);


                publicKeyGotadi = publicKeyGotadi.Replace("\r", "").Replace("\n", "\n");
                privateKeyGotadi = privateKeyGotadi.Replace("\r", "").Replace("\n", "\n");

                //RSACryptoServiceProvider gtdPublicKey = RSAHelper.GetCryptoServiceByPublicKey(Convert.FromBase64String(RSAHelper.GetPublicKey(GOTADI_CONFIG.Certificates.Gotadi_Public_Key)));
                //RSACryptoServiceProvider gtdPrivateKey = RSAHelper.GetCryptoServiceByPrivateKey(Convert.FromBase64String(RSAHelper.GetPrivateKey(GOTADI_CONFIG.Certificates.AEON_BTA_Private_Key)));

                string keyCommit = "ZO-l9YyW-svaSBOneyFHByZxqiMt4f0eDlflo5vfS8BoVbMjStm4qxhbFuow8JQz2L79YimyYBK9CIsobQX0wubNTI_6XDfpBKkJ7hBEGx7x2aIEcXjF6Twun_fHdJ44XXn1_9nen0DUdTpDnjs4NIQLMD-Z1mkwgijtYHpd8qk";
                string dataCommit = "3SRp74VaZRfQg4Yldx-KV2hVRbeUgmVtsRYq0MQy0TFMWv2ityPJ9l2JRcEwATaSJaU7yVii3CkcEqos8fm_Qw5C_uce43olJI6mK3oGzYzwpdwnpFf-Y1rGdZnDEVAm-A5obQCysKrIvd-aH9MbgrI5oXkOHr6Hii8q2USAgGWpnzrhf_yu0wuWJ6-aQWa6enyyz1jVBJ8uT48keZFf9SxvW68-fYUXkRiCd97m9RQO7ESjkfp6pudk-JfHzwonii8q2USAgGWpnzrhf_yu0wuWJ6-aQWa6VBVW8AyqyN-pfpZVIq0JI_9z583ehXGbW9g2lP12-fDroTAz_1KkqWUCjDNEn-LoOTliaxjDuB2Na7LEnBnEaZ0zuA2ZoZ_iGKedmn-aQoxtrg5nzZf56JtxtGf1XRfp1EeSlUC5TBz2_HbwdGlnpnPpl2J2sNjHiSMSJj3a4LCvABNRGAHCzpKcbkzyCjPgW6xbT5HkfCGBwyXmTe3za-qtiktQnW3dCCDokF-Gnl4-o93KEX7_MLFFFLwfcIysaJft8cwpz-s71Gxxlgm5ZnmyyGv9yvZo0YLQ91YFhfzTXbDVlqueeNyAxpyIVfzJFhGaxWrLZtbeXiPp5wdRTPoGpBHTO2Wcay1WtdF-s9elsziQbl1t7nGiNexAknSg0nXl3t3ZbMW2fTtUF3av3FOskMiaJqQzTTF8shRs5C0l8fvpuD-tI9PmK-cC1IFrtpGZ7sOYTmHapGEYGAgE2ytjpDO0hr1-R0csGkYS4vBuFS2xTE3Q_j0mEV1pSQqiTxD5HX8bgdsluw80qsqyUzPpTA7Qgd3Vv4VqUZtvPZtrCepkOr_RHP7F94-QFg6sRqkzKTVEaQX17v12cZyPBmFdB0GTriGBa1MeyrqJHC3q9OPSOic7AWLxJhlPV0lhnwQAMTHNV2htVMf_o5CLUunRiv51EsP6Q3c2kGfQ2sttyzIeHa6QWs1k-ENQs1Ir4SOeBWtQBX_CPRMdP6uaxdzszlc2QwzRzFSYO8TQ6WU";

                keyCommit = model.Key;
                dataCommit = model.Data;


                keyCommit = ConvertBase64(keyCommit);

                RSA privateKey = CreateCipherDecrypt(privateKeyGotadi);
                byte[] decryptedBytes = DecryptRSAToByte(keyCommit, privateKey);


                // byte[] randomKey = DecryptRSAToByte(keyCommit, privateKeyGotadi);

                dataCommit = ConvertBase64(dataCommit);
                string originalData = CryptoUtils.DecryptTripleDes(dataCommit, decryptedBytes);

                // Step 1: Decrypt random 3DES key.
                // byte[] randomKey = CryptoUtils.DecryptRSAToByte(keyCommit, privateKey);

                // Step 2: Encrypt data
                //String originalData = CryptoUtils.DecryptTripleDes(dataCommit, randomKey);

                /*string keyCommit = "kAJ5q47p_LtXF1ur6CQYQclYImPFaVAohcTZVfVEpIbTgpPwtBGmj8GB1sRbPQPaeetW2VUzkA0hqXwmDLAEsmRj83tB7AOk8SC56olTtbDi8EFKsZ2mGBU-oChtdiwChJB7zIEBx4pMu5dcUKojDMDBYUcC-ALLlfr3zbZJuO-gguqL3E4FPw7eXZ-jbaSYZK5EabdbVOqg0bJyzTqJbIUqyjMuc6yr2gP1M9rPWhQHmsZClLnGqM2Tvvmz2Tddlfr3zbZJuO-gguqL3E4FPw7eXZ-jbaSYh2-2gN5HblKs7aaHKd5INO9yJ4r9IUabSm3ko27dlAhGa06kg8eJGeuU0_nF5VCiwRNqNTJibU55gE0AN1lguyTzVxZJ2R77nerjeBuTVCLP_fVgaNe4UE0nNaF_5cTqe0c7GGf52oMuY9YOOnswpZqsT5xKE9ErrNQJf2VrwT5XwdSUiPCa-bpH725uow2HTvq5MZkGXZE4P2NaWqlRrQBaK9PmMzUWD5r1raKFDgN-TU_Jj0UC5SoYY5zrT5JRud9VQkHYKO5cAKAwjS1aTEVmZxyBdqcZTu5QgXMbZ01uDhO7UAyJQpo9iewMEubUaHeHowsdyaO0iv2O4zSJ9LDvyG4IpZSIN2Fezpt3PkL_bIWOWNg2t8DKyhFrpTa7FYsjAt1ol-GyfndbrLzbl0n85CqcPyujFvuEvmihpLymXIwDgSuAznrtLBRAK_FcCC73XM0LDcMuU4vWV3_3Hng9x4CW_ZmpMuaMDEF4hH3urRiVple9Dvu0TWzUR_tQxNNGmG-pq5IQ1a7zaWC2eXp1r75J6miWXeIn0BeBbFKTpYmBbcAwuFWDURCxnS5aiEqIGaKIbvZTdd7bPZoRsW1H6Q2J2zjUnNveKlNhceObNqw686G8jOlQ7cyztv3RTNr8Y8lG9aX0kuV6xYNrxfS87iB9hPdelcIFmr_BamKB76FJzzBflR8FEIwQKvRMUqwsdVsHbHY5Afpbyo4QEbZtdsHOtJe9";
                string dataCommit = "H3GiyHYt939bspX_34j5DoLOUjYg2oFQYrZ2UGInnMu-bUsj-6QHpeywS1znAYU26m1CKzm8pODgURvAfWNwXg2xMbQo5sKigxmg8orEK1aJhwg3jr0DRC5k7pxpqvFf4lOQN8lpBxGDE50bVMtuA3Q8YaQYBPYSDBZHvRQy1CI";

                this.gotadiPublicKey = CryptoUtils.getPublicKeyFromXML(System.getenv("GTD_PUBLIC_KEY"));
                this.privateKey = CryptoUtils.getPrivateKeyFromXML(System.getenv("PRIVATE_KEY"));*/


                var list = originalData.Split('|').ToList();


                /*PromoteAndTransferJob job = new PromoteAndTransferJob(_logger, unitOf, _settingBO);
                await job.Sync();*/
                return new ResultDTO { Object = originalData, Messages = new List<string>() { list[2].ToString() }};
            }
            catch (TaskCanceledException ex)
            {
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> RemoveLogRequestToHireImportTracking(CreatePayloadArgs args)
        {
            try
            {
                HashSet<ModelTest> Ids = new HashSet<ModelTest>();
                if (args != null && args.ReferenceNumbers.Any())
                {
                    foreach(var refer in args.ReferenceNumbers)
                    {
                        var importTrackings = await unitOf.GetRepository<ImportTracking>().FindByAsync(x => x.JsonDataStr.Contains(refer));
                        if (importTrackings != null)
                        {
                            foreach (var importTrackingDetail in importTrackings)
                            {
                                List<ImportRequestToHireError> jsonDataParsed = JsonConvert.DeserializeObject<List<ImportRequestToHireError>>(importTrackingDetail.JsonDataStr);
                                if (jsonDataParsed.Any() && jsonDataParsed.Any(x => !string.IsNullOrEmpty(x.ReferenceNumber) && !string.IsNullOrEmpty(x.Status) && x.ReferenceNumber.Equals(refer) && x.Status.ToLower().Equals("Success".ToLower())))
                                {
                                    jsonDataParsed = jsonDataParsed.Where(x => x.ReferenceNumber != refer).ToList();
                                    importTrackingDetail.JsonDataStr = JsonConvert.SerializeObject(jsonDataParsed);
                                    unitOf.GetRepository<ImportTracking>().Update(importTrackingDetail);
                                    Ids.Add(new ModelTest() { Id = importTrackingDetail.Id,  FileName = importTrackingDetail.FileName, ReferenceNumber = refer });
                                }
                            }
                        }
                    }
                    await unitOf.CommitAsync();
                }
                return new ResultDTO { Object = Ids };
            }
            catch (TaskCanceledException ex)
            {
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }

        [HttpGet]
        public async Task<ResultDTO> JobManualMoveUserPromoteAndTransfer()
        {
            try
            {
                PromoteAndTransferJob job = new PromoteAndTransferJob(_logger, unitOf, _settingBO);
                return new ResultDTO { Object = await job.Sync() };
            }
            catch (TaskCanceledException ex)
            {
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }

        [HttpGet]
        public async Task<ResultDTO> TestEmailNotification()
        {
            try
            {
                var mergeFields = new Dictionary<string, string>();
                mergeFields["ApproverName"] = "1ˢᵗ Approver";
                var recipients = new List<string>() { "phavatam@gmail.com" };
                var ccRecipients = new List<string>();
                await _emailNotificationBO.SendEmail(EmailTemplateName.For1STResignation , EmailTemplateName.ForAssigneeCompleted, mergeFields, recipients);
                _logger.LogInformation("TestEmailNotification");
                return new ResultDTO { };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at ReTry", ex.Message);
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetUsers(UserSAPArg arg)
        {
            try
            {
                return await _employee.GetUsers(arg);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at ReTry", ex.Message);
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> CreateF2Form()
        {
            try
            {
                return await _edoc01BO.CreateF2Form();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at CreateF2Form", ex.Message);
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }
        [HttpGet]
        public async Task<bool> TestPushJob(Guid id)
        {
            RequestToHireCompleteAction action = new RequestToHireCompleteAction();
            await action.Execute(unitOf, id, _dashboardBO, _workflowBO, _logger);
            return true;
        }
        [HttpGet]
        public async Task<bool> TestCompletedBTAItem(Guid id)
        {
            BTACompleteAction action = new BTACompleteAction();
            await action.Execute(unitOf, id, _dashboardBO, _workflowBO, _logger);
            return true;
        }
        [HttpPost]
        public async Task<object> TestTargetPlan(object json)
        {
            string url = "/sap/opu/odata/SAP/ZHCM_GW_INTE_TARGETPLANS_SRV/TargetPlansSet";
            return await _bo.TestPushTargetPlanToSAP(url, json.ToString());
        }
        [HttpGet]
        public async Task<ResultDTO> GetShiftSetByDate(string SAPCode, DateTime date)
        {
            try
            {
                ShiftSetResponceSAPViewModel returnValue = new ShiftSetResponceSAPViewModel();
                var predicate = "$filter=Pernr eq ('{0}') and Date eq ('{1}')";
                var predicateParameters = new string[] { SAPCode , date .ToString("yyyyMMdd")};
                var excution = _externalServiceBO.BuildAPIService(ExtertalType.ShiftSet);
                excution.APIName = "GetCurrentShiftSet";
                var res = await excution.GetData(predicate, predicateParameters);
                excution.APIName = string.Empty;

                if(res != null && res is List<ShiftSetResponceSAPViewModel>)
                {
                    List<ShiftSetResponceSAPViewModel> array = (List<ShiftSetResponceSAPViewModel>)res;
                    returnValue = array.FirstOrDefault();
                }

                return new ResultDTO { Object = returnValue };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetShiftSetByDate", ex.Message + ex.StackTrace);
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }
        [HttpPost]
        public async Task<ResultDTO> GetShiftSetArrayByDate(List<ShiftCodeByDateArg> args)
        {
            try
            {
                if (args.Any())
                {
                    List<ShiftSetResponceSAPViewModel> returnData = new List<ShiftSetResponceSAPViewModel>();
                    var dateGroups = args.GroupBy(g => g.ExchangeDate).Select(s => s.Key);
                    if (dateGroups.Any())
                    {
                        #region Functs
                        Func<IEnumerable<string>, string> prepareFilterStatement = (IEnumerable<string> sapCodes) =>
                        {
                            string returnValue = string.Empty;
                            if(sapCodes.Any())
                            {
                                returnValue = sapCodes
                                .Where(x => !string.IsNullOrEmpty(x))
                                .Select(x => $"Pernr eq ('{x}')")
                                .Aggregate((x, y) => x + " or " + y);
                            }
                            return returnValue;
                        };
                        #endregion

                        var excution = _externalServiceBO.BuildAPIService(ExtertalType.ShiftSet);
                        excution.APIName = "GetCurrentShiftSet";
                        foreach (var cDate in dateGroups)
                        {
                            var sapCodes = args.Where(i => i.ExchangeDate == cDate).GroupBy(g => g.SapCode).Select(s => s.Key).ToList();
                            if(sapCodes.Any())
                            {
                                string filterStatement = prepareFilterStatement(sapCodes);
                                var predicate = $"$filter=({filterStatement}) and Date eq ('{cDate}')"; 
                                var res = await excution.GetData(predicate, new string[] { });
                                if(res != null && res is List<ShiftSetResponceSAPViewModel>)
                                {
                                    returnData.AddRange((List<ShiftSetResponceSAPViewModel>)res);
                                }
                            }
                           
                        }
                        excution.APIName = string.Empty;
                        return new ResultDTO { Object = returnData };
                    }
                }
                return new ResultDTO { ErrorCodes = { 1002 }, Messages = { "No Data" } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at GetShiftSetByDate", ex.Message + ex.StackTrace);
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        } 
        [HttpGet]
        public async Task<ResultDTO> TestAPIRes()
        {
            try
            {
                SendMail1STResignations job = new SendMail1STResignations(_logger, unitOf, _email);
                job.SendNotifications();
                _logger.LogInformation("TestEmailNotification");

                return new ResultDTO {  };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at method: TestAPI", ex.Message);
                return new ResultDTO { Object = ex, ErrorCodes = { 1002 }, Messages = { "Error System" } };
            }
        }

        [HttpGet]
        public async Task<ResultDTO> SyncWorkflowChangeRequestedJobGrade()
        {
            try
            {
                HashSet<string> fieldName = new HashSet<string>();
                var allWorkflowList = await unitOf.GetRepository<WorkflowTemplate>(true).GetAllAsync();
                List<Guid> templateIds = new List<Guid>();
                if (allWorkflowList.Any())
                {
                    foreach(var item in allWorkflowList)
                    {   
                        var currentItem = item;
                        bool isUpdateWf = false;

                        if (currentItem.WorkflowData != null)
                        {
                            if (currentItem.WorkflowData.StartWorkflowConditions.Any())
                            {
                                foreach (var condition in currentItem.WorkflowData.StartWorkflowConditions)
                                {
                                    if (!string.IsNullOrEmpty(condition.FieldName) && Const.FieldNameGrades.Contains(condition.FieldName))
                                    {
                                        //condition.FieldValues = condition.FieldValues.Select(x => x.Replace("G", "")).ToList();
                                        List<string> fieldValues = new List<string>();
                                        foreach (var str in condition.FieldValues)
                                        {
                                            var grade = str;
                                            if (grade.Contains("G"))
                                            {
                                                grade = grade.Replace("G", "");
                                                int currentGrade = int.Parse(grade);
                                                if (currentGrade > 5)
                                                {
                                                    currentGrade += 1;
                                                }
                                                grade = currentGrade.ToString();
                                            }
                                            fieldValues.Add(grade);
                                        }
                                        condition.FieldValues = fieldValues;
                                        templateIds.Add(item.Id);
                                        isUpdateWf = true;
                                    }
                                }
                            }
                            if (currentItem.WorkflowData.Steps.Any())
                            {
                                foreach(var step in currentItem.WorkflowData.Steps)
                                {
                                    if (!string.IsNullOrEmpty(step.JobGrade))
                                    {
                                        step.JobGrade = step.JobGrade.Replace("G", "");
                                        int currentJobGrade = int.Parse(step.JobGrade);
                                        if (currentJobGrade > 5)
                                        {
                                            currentJobGrade += 1;
                                        }
                                        step.JobGrade = currentJobGrade.ToString();
                                        templateIds.Add(item.Id);
                                        isUpdateWf = true;
                                    }
                                    if (!string.IsNullOrEmpty(step.MaxJobGrade))
                                    {
                                        step.MaxJobGrade = step.MaxJobGrade.Replace("G", "");
                                        int currentJobGrade = int.Parse(step.MaxJobGrade);
                                        if (currentJobGrade > 5)
                                        {
                                            currentJobGrade += 1;
                                        }
                                        step.MaxJobGrade = currentJobGrade.ToString();
                                        templateIds.Add(item.Id);
                                        isUpdateWf = true;
                                    }
                                }
                            }
                        }
                        if (isUpdateWf)
                        {
                            item.WorkflowDataStr = JsonConvert.SerializeObject(currentItem.WorkflowData);
                            unitOf.GetRepository<WorkflowTemplate>().Update(item);
                        }
                    }
                    await unitOf.CommitAsync();
                }
                return new ResultDTO { Object = templateIds };
            }
            catch (TaskCanceledException ex)
            {
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }

        [HttpGet]
        public async Task<ResultDTO> SyncWorkflowChangeTicketInprocessing()
        {
            try
            {
                HashSet<string> fieldName = new HashSet<string>();
                int toYear = DateTimeOffset.Now.Year;
                var allWorkflowInstanceList = await unitOf.GetRepository<WorkflowInstance>(true).FindByAsync(x => !x.IsCompleted);
                var allWorkflowTemplate = await unitOf.GetRepository<WorkflowTemplate>(true).GetAllAsync();
                List<string> templateIds = new List<string>();
                if (allWorkflowInstanceList.Any())
                {
                    foreach (var item in allWorkflowInstanceList)
                    {

                        foreach( var instance in allWorkflowInstanceList)
                        {
                            var workflowTemplate = allWorkflowTemplate.Where(x => x.Id == instance.TemplateId).FirstOrDefault();
                            if (!(workflowTemplate is null))
                            {
                                instance.WorkflowDataStr = workflowTemplate.WorkflowDataStr;
                                templateIds.Add(instance.ItemReferenceNumber);
                            }
                        }
                    }
                    await unitOf.CommitAsync();
                }
                return new ResultDTO { Object = templateIds };
            }
            catch (TaskCanceledException ex)
            {
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }
        [HttpGet]
        public async Task<ResultDTO> SyncWorkflowUpgradeOldG5Process()
        {
            try
            {
                HashSet<string> fieldName = new HashSet<string>();
                HashSet<Guid> templateIds = new HashSet<Guid>();
                int toYear = DateTimeOffset.Now.Year;
                var allWorkflowList = await unitOf.GetRepository<WorkflowTemplate>(true).GetAllAsync();
                if (allWorkflowList.Any())
                {
                    foreach (var item in allWorkflowList)
                    {
                        if (item.WorkflowData.StartWorkflowConditions.Any())
                        {
                            WorkflowCondition requestedJobgrade = item.WorkflowData.StartWorkflowConditions.Where(x => Const.FieldNameGrades.Contains(x.FieldName)).FirstOrDefault();
                            if (!(requestedJobgrade is null))
                            {
                                if ((requestedJobgrade.FieldValues.Contains("5"))) {
                                    IList<string> reqValue = requestedJobgrade.FieldValues;
                                    if (!reqValue.Contains("6"))
                                    {
                                        reqValue.Add("6");
                                    }
                                    requestedJobgrade.FieldValues = reqValue.OrderBy(y => y).ToList();
                                    item.WorkflowData.StartWorkflowConditions.Remove(requestedJobgrade);
                                    item.WorkflowData.StartWorkflowConditions.Add(requestedJobgrade);

                                    foreach (var step in item.WorkflowData.Steps)
                                    {
                                        if (step.MaxJobGrade == "5")
                                        {
                                            step.MaxJobGrade = "6";
                                            templateIds.Add(item.Id);
                                        }
                                    }

                                    item.WorkflowDataStr = JsonConvert.SerializeObject(item.WorkflowData);
                                    unitOf.GetRepository<WorkflowTemplate>().Update(item);
                                }
                            }
                        }
                    }
                    await unitOf.CommitAsync();
                }
                return new ResultDTO { Object = templateIds };
            }
            catch (TaskCanceledException ex)
            {
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }


        [HttpGet]
        public async Task<ResultDTO> SyncWorkflowParticipantTypeUpper()
        {
            try
            {
                HashSet<string> fieldName = new HashSet<string>();
                int toYear = DateTimeOffset.Now.Year;
                var allWorkflowList = await unitOf.GetRepository<WorkflowTemplate>(true).GetAllAsync();
                List<Guid> templateIds = new List<Guid>();
                List<string> grades = new List<string>() { "7", "8", "9" };
                if (allWorkflowList.Any())
                {
                    foreach (var item in allWorkflowList)
                    {
                        if (!(item.WorkflowData is null) && !(item.WorkflowData.Steps is null))
                        {
                            int countStep7 = item.WorkflowData.Steps.Where(x => x.ParticipantType == ParticipantType.UpperDepartment && !string.IsNullOrEmpty(x.JobGrade) && x.JobGrade == "7").Count();
                            if (countStep7 > 0)
                            {
                                foreach (var step in item.WorkflowData.Steps)
                                {
                                    if (step.ParticipantType == ParticipantType.UpperDepartment && grades.Contains(step.JobGrade))
                                    {
                                        int currentJobGrade = int.Parse(step.JobGrade);
                                        currentJobGrade = (currentJobGrade - 1);
                                        step.JobGrade = currentJobGrade.ToString();
                                        templateIds.Add(item.Id);
                                    }
                                }
                                item.WorkflowDataStr = JsonConvert.SerializeObject(item.WorkflowData);
                            }
                        }
                    }
                    await unitOf.CommitAsync();
                }
                return new ResultDTO { Object = templateIds };
            }
            catch (TaskCanceledException ex)
            {
                return new ResultDTO { ErrorCodes = { 1004 }, Messages = { ex.Message } };
            }
        }

        [HttpPost]
        public async Task<ResultDTO> GetTodoList(TodoListArgs args)
        {
            return await _workflowBO.GetTodoList(args);
        }

        [HttpPost]
        public async Task<ResultDTO> ResetMultiplePassword(ModelResetMultiplePassword args)
        {
            var result = new ResultDTO();
            if (args.UserIds != null && args.UserIds.Any())
            {
                var userSuccess = new List<Guid>();
                foreach (var userId in args.UserIds)
                {
                    await _settingBO.ResetPassword(userId);
                    userSuccess.Add(userId);
                }
                result.Object = userSuccess;
            }
            return result;
        }
    }

    public class ModelTest
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ReferenceNumber { get; set; }
    }

    public class ModelResetMultiplePassword
    {
        public List<Guid> UserIds { get; set; }
    }

    public class ModelTestJob
    {
        public string Key { get; set; }
        public string Data { get; set; }
    }
}
