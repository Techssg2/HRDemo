using AutoMapper;
using Aeon.HR.ViewModels.Args;
using Aeon.HR.ViewModels.DTOs;
using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Aeon.HR.Infrastructure.Enums;
using Aeon.HR.BusinessObjects.Interfaces;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Aeon.HR.BusinessObjects.Helpers;
using System.Net;
using Newtonsoft.Json.Converters;

namespace Aeon.HR.BusinessObjects.Handlers
{
    public partial class RecruitmentBO
    {
        public RecruitmentBO()
        {

        }
        public async Task<ArrayResultDTO> GetApplicantList(QueryArgs args)
        {
            var items = await _uow.GetRepository<Applicant>().FindByAsync<ApplicantViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);

            var count = await _uow.GetRepository<Applicant>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = items, Count = count };
            return result;
        }
        public async Task<ArrayResultDTO> GetSimpleApplicantList(QueryArgs args)
        {
            var items = await _uow.GetRepository<Applicant>().FindByAsync<SimpleApplicantViewModel>(args.Order, args.Page, args.Limit, args.Predicate, args.PredicateParameters);

            var count = await _uow.GetRepository<Applicant>().CountAsync(args.Predicate, args.PredicateParameters);
            var result = new ArrayResultDTO { Data = items, Count = count };
            return result;
        }

        public async Task<ResultDTO> CreateApplicant(ApplicantArgs data)
        {
            var applicant = Mapper.Map<Applicant>(data);
            applicant.ApplicantRelativeInAeons = null;
            applicant.ApplicantEducations = null;
            applicant.ApplicantWorkingProcesses = null;
            var initStatus = await _uow.GetRepository<ApplicantStatusRecruitment>().FindByAsync<ApplicantStatusRecruitmentViewModel>(x => x.Name == "Initial" || x.Code == "Initial" || x.Code == "AppStatus1" || x.Id == new Guid("F17CE834-F0A7-4EDE-B724-4940E682A67A"));
            if (initStatus != null && initStatus.Any())
            {
                applicant.ApplicantStatusId = initStatus.FirstOrDefault().Id;
            }

            _uow.GetRepository<Applicant>().Add(applicant);

            if (data.ApplicantRelativeInAeons != null && data.ApplicantRelativeInAeons.Any())
            {
                var existApplicantRelativeInAeons = await _uow.GetRepository<ApplicantRelativeInAeon>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<ApplicantRelativeInAeon>().Delete(existApplicantRelativeInAeons);
                foreach (var _model in data.ApplicantRelativeInAeons)
                {
                    _model.ApplicantId = applicant.Id;
                    _uow.GetRepository<ApplicantRelativeInAeon>().Add(_model);
                }
            }

            if (data.ApplicantEducations != null && data.ApplicantEducations.Any())
            {
                var existApplicantEducations = await _uow.GetRepository<ApplicantEducation>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<ApplicantEducation>().Delete(existApplicantEducations);
                foreach (var _model in data.ApplicantEducations)
                {
                    _model.ApplicantId = applicant.Id;
                    _uow.GetRepository<ApplicantEducation>().Add(_model);
                }
            }
            if (data.ApplicantWorkingProcesses != null && data.ApplicantWorkingProcesses.Any())
            {
                var existApplicantWorkingProcesses = await _uow.GetRepository<ApplicantWorkingProcess>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<ApplicantWorkingProcess>().Delete(existApplicantWorkingProcesses);
                foreach (var _model in data.ApplicantWorkingProcesses)
                {
                    _model.ApplicantId = applicant.Id;
                    _uow.GetRepository<ApplicantWorkingProcess>().Add(_model);
                }
            }
            if (data.FamilyMembers != null && data.FamilyMembers.Any())
            {
                var familyMembers = await _uow.GetRepository<FamilyMember>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<FamilyMember>().Delete(familyMembers);
                foreach (var _model in data.FamilyMembers)
                {
                    _model.ApplicantId = applicant.Id;
                    _uow.GetRepository<FamilyMember>().Add(_model);
                }
            }
            if (data.EmploymentHistories != null && data.EmploymentHistories.Any())
            {
                var employmentHistories = await _uow.GetRepository<EmploymentHistory>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<EmploymentHistory>().Delete(employmentHistories);
                foreach (var _model in data.EmploymentHistories)
                {
                    _model.ApplicantId = applicant.Id;
                    _uow.GetRepository<EmploymentHistory>().Add(_model);
                }
            }
            if (data.CharacterReferences != null && data.CharacterReferences.Any())
            {
                var characterReferences = await _uow.GetRepository<CharacterReferee>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<CharacterReferee>().Delete(characterReferences);
                foreach (var _model in data.CharacterReferences)
                {
                    _model.ApplicantId = applicant.Id;
                    _uow.GetRepository<CharacterReferee>().Add(_model);
                }
            }
            if (data.LanguageProficiencyEntries != null && data.LanguageProficiencyEntries.Any())
            {
                var languageProficiencyEntries = await _uow.GetRepository<LanguageProficiencyEntry>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<LanguageProficiencyEntry>().Delete(languageProficiencyEntries);
                foreach (var _model in data.LanguageProficiencyEntries)
                {
                    _model.ApplicantId = applicant.Id;
                    _uow.GetRepository<LanguageProficiencyEntry>().Add(_model);
                }
            }
            if (data.Activities != null && data.Activities.Any())
            {
                var activities = await _uow.GetRepository<Activity>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<Activity>().Delete(activities);
                foreach (var _model in data.Activities)
                {
                    _model.ApplicantId = applicant.Id;
                    _uow.GetRepository<Activity>().Add(_model);
                }
            }



            //var status = await _uow.GetRepository<ApplicantStatusRecruitment>().FindByAsync(x => x.Code.Equals("AppStatus1"));
            //if (status != null)
            //{
            //    applicant.ApplicantStatusId = status.FirstOrDefault()?.Id;
            //}

            var status = await _uow.GetRepository<ApplicantStatusRecruitment>().FindByAsync(x => x.Code.Equals("APP1"));
            if (status.Any())
            {
                applicant.ApplicantStatusId = status.FirstOrDefault()?.Id;
            }

            await _uow.CommitAsync();
            return new ResultDTO { Object = Mapper.Map<ApplicantViewModel>(applicant) };
        }

        public async Task<ResultDTO> UpdateApplicant(ApplicantArgs data)
        {
            var existApplicant = await _uow.GetRepository<Applicant>().AnyAsync(x => x.Id == data.Id);
            if (!existApplicant)
            {
                return new ResultDTO
                {
                    ErrorCodes = { 404 },
                    Messages = { $"Applicant with id {data.Id} is not found!" },
                };
            }
            var currentApplicant = Mapper.Map<Applicant>(data);

            //existApplicant.FullName = data.FullName;
            //existApplicant.GenderCode = data.GenderCode;
            //existApplicant.GenderName = data.GenderName;
            //existApplicant.NationalityCode = data.NationalityCode;
            //existApplicant.NationalityName = data.NationalityName;
            //existApplicant.Mobile = data.Mobile;
            //existApplicant.HasEmailAddress = data.HasEmailAddress;
            //existApplicant.Email = data.Email;
            //existApplicant.DateOfBirth = data.DateOfBirth;
            //existApplicant.NativeCode = data.NativeCode;
            //existApplicant.NativeName = data.NativeName;
            //existApplicant.BirthPlaceCode = data.BirthPlaceCode;
            //existApplicant.BirthPlaceName = data.BirthPlaceName;
            //existApplicant.HaveIdentifyCardNumber12 = data.HaveIdentifyCardNumber12;
            //existApplicant.IDCard9Number = data.IDCard9Number;
            //existApplicant.IDCard9Date = data.IDCard9Date;
            //existApplicant.IDCard9PlaceCode = data.IDCard9PlaceCode;
            //existApplicant.IDCard9PlaceName = data.IDCard9PlaceName;
            //existApplicant.IDCard12Number = data.IDCard12Number;
            //existApplicant.IDCard12Date = data.IDCard12Date;
            //existApplicant.IDCard12PlaceCode = data.IDCard12PlaceCode;
            //existApplicant.IDCard12PlaceName = data.IDCard12PlaceName;

            //existApplicant.PermanentResidentAddress = data.PermanentResidentAddress;
            //existApplicant.PermanentResidentCityCode = data.PermanentResidentCityCode;
            //existApplicant.PermanentResidentCityName = data.PermanentResidentCityName;
            //existApplicant.PermanentResidentDistrictCode = data.PermanentResidentDistrictCode;
            //existApplicant.PermanentResidentDistrictName = data.PermanentResidentDistrictName;
            //existApplicant.PermanentResidentWardCode = data.PermanentResidentWardCode;
            //existApplicant.PermanentResidentWardName = data.PermanentResidentWardName;

            //existApplicant.ProvisionalResidentAddress = data.ProvisionalResidentAddress;
            //existApplicant.ProvisionalResidentCityCode = data.ProvisionalResidentCityCode;
            //existApplicant.ProvisionalResidentCityName = data.ProvisionalResidentCityName;
            //existApplicant.ProvisionalResidentDistrictCode = data.ProvisionalResidentDistrictCode;
            //existApplicant.ProvisionalResidentDistrictName = data.ProvisionalResidentDistrictName;
            //existApplicant.ProvisionalResidentWardCode = data.ProvisionalResidentWardCode;
            //existApplicant.ProvisionalResidentWardName = data.ProvisionalResidentWardName;
            //existApplicant.HasConvictedForCrime = data.HasConvictedForCrime;
            //existApplicant.ConvictedForCrimeNotes = data.ConvictedForCrimeNotes;
            //existApplicant.HasHealthProblem = data.HasHealthProblem;
            //existApplicant.InformationAboutHealthProblem = data.InformationAboutHealthProblem;
            //existApplicant.MaritalStatus = data.MaritalStatus;
            //existApplicant.HasPregnant = data.HasPregnant;
            //existApplicant.HaveChildrenUnder12Month = data.HaveChildrenUnder12Month;
            //existApplicant.HaveRelativeInAeon = data.HaveRelativeInAeon;
            //existApplicant.ReligionCode = data.ReligionCode;
            //existApplicant.ReligionName = data.ReligionName;
            //existApplicant.HavePassport = data.HavePassport;
            //existApplicant.PassportNo = data.Passport;            
            //existApplicant.PassportType = data.PassportType;
            //if(existApplicant.Type == ApplicantType.G2Up)
            //{
            //    existApplicant.Position1Name = data.Position1Name;
            //    existApplicant.Position2Name = data.Position2Name;
            //    existApplicant.GrossSalary1 = data.GrossSalary1;
            //    existApplicant.GrossSalary2 = data.GrossSalary2;
            //    existApplicant.TerminationNotice1 = data.TerminationNotice1;
            //    existApplicant.TerminationNotice2 = data.TerminationNotice2;        

            //}
            if (data.ApplicantRelativeInAeons != null && data.ApplicantRelativeInAeons.Any())
            {
                var existApplicantRelativeInAeons = await _uow.GetRepository<ApplicantRelativeInAeon>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<ApplicantRelativeInAeon>().Delete(existApplicantRelativeInAeons);
                foreach (var _model in data.ApplicantRelativeInAeons)
                {
                    _model.ApplicantId = data.Id;
                    _uow.GetRepository<ApplicantRelativeInAeon>().Add(_model);
                }
            }

            if (data.ApplicantEducations != null && data.ApplicantEducations.Any())
            {
                var existApplicantEducations = await _uow.GetRepository<ApplicantEducation>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<ApplicantEducation>().Delete(existApplicantEducations);
                foreach (var _model in data.ApplicantEducations)
                {
                    _model.ApplicantId = data.Id;
                    _uow.GetRepository<ApplicantEducation>().Add(_model);
                }
            }
            if (data.ApplicantWorkingProcesses != null && data.ApplicantWorkingProcesses.Any())
            {
                var existApplicantWorkingProcesses = await _uow.GetRepository<ApplicantWorkingProcess>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<ApplicantWorkingProcess>().Delete(existApplicantWorkingProcesses);
                foreach (var _model in data.ApplicantWorkingProcesses)
                {
                    _model.ApplicantId = data.Id;
                    _uow.GetRepository<ApplicantWorkingProcess>().Add(_model);
                }
            }
            if (data.FamilyMembers != null && data.FamilyMembers.Any())
            {
                var familyMembers = await _uow.GetRepository<FamilyMember>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<FamilyMember>().Delete(familyMembers);
                foreach (var _model in data.FamilyMembers)
                {
                    _model.ApplicantId = data.Id;
                    _uow.GetRepository<FamilyMember>().Add(_model);
                }
            }
            if (data.EmploymentHistories != null && data.EmploymentHistories.Any())
            {
                var employmentHistories = await _uow.GetRepository<EmploymentHistory>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<EmploymentHistory>().Delete(employmentHistories);
                foreach (var _model in data.EmploymentHistories)
                {
                    _model.ApplicantId = data.Id;
                    _uow.GetRepository<EmploymentHistory>().Add(_model);
                }
            }
            if (data.CharacterReferences != null && data.CharacterReferences.Any())
            {
                var characterReferences = await _uow.GetRepository<CharacterReferee>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<CharacterReferee>().Delete(characterReferences);
                foreach (var _model in data.CharacterReferences)
                {
                    _model.ApplicantId = data.Id;
                    _uow.GetRepository<CharacterReferee>().Add(_model);
                }
            }
            if (data.LanguageProficiencyEntries != null && data.LanguageProficiencyEntries.Any())
            {
                var languageProficiencyEntries = await _uow.GetRepository<LanguageProficiencyEntry>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<LanguageProficiencyEntry>().Delete(languageProficiencyEntries);
                foreach (var _model in data.LanguageProficiencyEntries)
                {
                    _model.ApplicantId = data.Id;
                    _uow.GetRepository<LanguageProficiencyEntry>().Add(_model);
                }
            }
            if (data.Activities != null && data.Activities.Any())
            {
                var activities = await _uow.GetRepository<Activity>().FindByAsync(x => x.ApplicantId == data.Id);
                _uow.GetRepository<Activity>().Delete(activities);
                foreach (var _model in data.Activities)
                {
                    _model.ApplicantId = data.Id;
                    _uow.GetRepository<Activity>().Add(_model);
                }
            }

            //existApplicant.WorkingTimeRecruitmentId = data.WorkingTimeRecruitmentId;
            //existApplicant.AppropriateTimeCode = data.AppropriateTimeCode;
            //existApplicant.AppropriateTimeName = data.AppropriateTimeName;
            //existApplicant.PositionId = data.PositionId;
            //existApplicant.HasExperienceOnWorkingInShift = data.HasExperienceOnWorkingInShift;
            //existApplicant.HasExperienceOnStandingAndWalking = data.HasExperienceOnStandingAndWalking;
            //existApplicant.HasExperienceOnRetail = data.HasExperienceOnRetail;
            //existApplicant.ExpectedStartingDateCode = data.ExpectedStartingDateCode;
            //existApplicant.ExpectedStartingDateName = data.ExpectedStartingDateName;
            //existApplicant.ExpectedStartingDateOther = data.ExpectedStartingDateOther;
            //existApplicant.FullTimeWorkingExprerimentCode = data.FullTimeWorkingExprerimentCode;
            //existApplicant.FullTimeWorkingExprerimentName = data.FullTimeWorkingExprerimentName;
            //existApplicant.CanWorkEarlyOrLateShift = data.CanWorkEarlyOrLateShift;
            //existApplicant.SkillsCode = data.SkillsCode;
            //existApplicant.SkillsName = data.SkillsName;
            //existApplicant.AbilitiesCode = data.AbilitiesCode;
            //existApplicant.AbilitiesName = data.AbilitiesName;
            //existApplicant.LanguagesCode = data.LanguagesCode;
            //existApplicant.LanguagesName = data.LanguagesName;

            //existApplicant.InterestsInWork1stPriorityCode = data.InterestsInWork1stPriorityCode;
            //existApplicant.InterestsInWork1stPriorityName = data.InterestsInWork1stPriorityName;
            //existApplicant.InterestsInWork2ndPriorityCode = data.InterestsInWork2ndPriorityCode;
            //existApplicant.InterestsInWork2ndPriorityName = data.InterestsInWork2ndPriorityName;
            //existApplicant.InterestsInWork3rdPriorityCode = data.InterestsInWork3rdPriorityCode;
            //existApplicant.InterestsInWork3rdPriorityName = data.InterestsInWork3rdPriorityName;
            //existApplicant.InterestsInWork4thPriorityCode = data.InterestsInWork4thPriorityCode;
            //existApplicant.InterestsInWork4thPriorityName = data.InterestsInWork4thPriorityName;
            //existApplicant.InterestsInWork5thPriorityCode = data.InterestsInWork5thPriorityCode;
            //existApplicant.InterestsInWork5thPriorityName = data.InterestsInWork5thPriorityName;
            //existApplicant.HaveAppliedInAeon = data.HaveAppliedInAeon;
            //existApplicant.HaveAppliedInAeonDetail = data.HaveAppliedInAeonDetail;
            //existApplicant.HaveWorkedInAeon = data.HaveWorkedInAeon;
            //existApplicant.ExpectedSalary = data.ExpectedSalary;0
            //existApplicant.KnowJobFromWeb = data.KnowJobFromWeb;
            //existApplicant.KnowJobFromSchool = data.KnowJobFromSchool;
            //existApplicant.KnowJobFromOthers = data.KnowJobFromOthers;
            _uow.GetRepository<Applicant>().Update(currentApplicant);
            await _uow.CommitAsync();

            return new ResultDTO { Object = Mapper.Map<ApplicantViewModel>(currentApplicant) };
        }

        public async Task<ResultDTO> DeleteApplicant(Guid id)
        {
            bool existApplicant = _uow.GetRepository<Applicant>().Any(x => x.Id == id);
            if (!existApplicant)
            {
                return new ResultDTO
                {

                    ErrorCodes = { 404 },
                    Messages = { $"Applicant id {id} is not found!" },
                };
            }
            else
            {
                var Applicant = _uow.GetRepository<Applicant>().FindById(id);
                await _uow.CommitAsync();
                return new ResultDTO
                {

                };
            }
        }

        public async Task<ResultDTO> UpdatePositionDetail(PositionDetailItemArgs data)
        {
            var existApplicant = await _uow.GetRepository<Applicant>().FindByIdAsync(data.Id);
            existApplicant.IsSignedOffer = data.IsSignedOffer;
            existApplicant.StartDate = data.StartDate;
            existApplicant.ApplicantStatusId = data.ApplicantStatusId;
            existApplicant.EmployeeGroupCode = data.EmployeeGroupCode;
            existApplicant.EmployeeSybGroupCode = data.EmployeeSybGroupCode;
            existApplicant.ReasonHiringCode = data.ReasonHiringCode;
            existApplicant.AppreciationId = data.AppreciationId;
            existApplicant.AdditionalPositionCode = data.AdditionalPositionCode;
            existApplicant.AdditionalPositionName = data.AdditionalPositionName;
            existApplicant.HasCreateF2Form = data.HasCreateF2Form;
            //var applicant = Mapper.Map<Applicant>(existApplicant);
            //_uow.GetRepository<Applicant>().Update(applicant);
            await _uow.CommitAsync();
            if (existApplicant.IsSignedOffer)
            {
                await _externalServiceBO.SubmitData(existApplicant, true);
            }
            return new ResultDTO { };
        }

        public async Task<ResultDTO> SearchApplicantList(ApplicantSearchArgs query)
        {
            //string keyword,Guid positionId,Guid departmentId,string genderCode,DateTime createdDateFrom,DateTime createdDateTo,bool getAll = false
            Thread.Sleep(1000);
            IEnumerable<Applicant> lstApplicant = null;
            var existUser = await _uow.GetRepository<User>().FindByAsync(x => x.SAPCode == query.sapCode);
            if (!existUser.Any())
            {
                return new ResultDTO
                {

                    ErrorCodes = { 404 },
                    Messages = { $"Employee with SAPCode {query.sapCode} does not exist" },
                };
            }
            try
            {
                if (query.getAll)
                {
                    lstApplicant = await _uow.GetRepository<Applicant>().FindByAsync("CreatedDate ASC", 1, 10, query.query.Predicate, query.query.PredicateParameters);
                }
                else
                {
                    var CurrentId = existUser.FirstOrDefault().Id;
                    //SharePoint need to be checked
                    //query.query.Predicate += " && CreatedById = @2 && IsDeleted = @3";
                    //var paramArray = query.query.PredicateParameters.Append(CurrentId).Append(false).ToArray();
                    lstApplicant = await _uow.GetRepository<Applicant>().FindByAsync("CreatedDate ASC", 1, 10, query.query.Predicate, query.query.PredicateParameters);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            List<ItemListApplicantViewModel> vmLstApplicant = Mapper.Map<List<ItemListApplicantViewModel>>(lstApplicant);
            ResultDTO result = new ResultDTO
            {

                Object = new ArrayResultDTO
                {
                    Data = vmLstApplicant,
                    Count = vmLstApplicant.Count
                },
            };
            return result;
        }
        public async Task<ResultDTO> SubmitApplicant(Guid Id)
        {
            var res = await _uow.GetRepository<Applicant>().GetSingleAsync(x => x.Id == Id);
            if (res != null)
            {
                await _externalServiceBO.SubmitData(res, true);
            }

            return new ResultDTO { Object = Mapper.Map<ApplicantViewModel>(res) };

        }
        protected HttpClient ConfigAPI(string url, string userName, string pass)
        {
            var credentials = new NetworkCredential(userName, pass);
            var handler = new HttpClientHandler { Credentials = credentials };
            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;

        }
        private string ReplaceNull(string iText)
        {
            var result = iText.Replace("null", "\"\"");
            return result;
        }
        public async Task<ResultDTO> SearchApplicant(QueryArgs queryArg)
        {
            var result = new ResultDTO();
            var aeonAccount = ConfigurationManager.AppSettings["massAccount"];
            var aeonPassword = ConfigurationManager.AppSettings["massPassword"];
            var baseUrl = ConfigurationManager.AppSettings["massUrl"];
            var url = string.Format("{0}/{1}", baseUrl, "Data/QueryData?entity=ApplicationFormListViewModel");
            var client = ConfigAPI(url, aeonAccount, aeonPassword);

            var payload = JsonConvert.SerializeObject(queryArg);
            payload = ReplaceNull(payload);
            var content = Utilities.StringContentObjectFromJson(payload);
            try
            {
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var format = "yyyy-MM-ddTHH:mm:ssZ"; // your datetime format
                    var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                    var resultArray = JsonConvert.DeserializeObject<ApplicantFromMassViewModel>(httpResponseResult, dateTimeConverter);
                    if (resultArray != null)
                    {
                        result.Object = new ArrayResultDTO { Data = resultArray, Count = resultArray.Count };
                    }
                }

            }
            catch (Exception ex)
            {
                result.ErrorCodes.Add(1004);
                result.Messages.Add("Something went wrong");
                result.Messages.Add(ex.Message);
            }

            return result;

        }
        public async Task<ResultDTO> GetDetailApplicant(Guid id)
        {
            var result = new ResultDTO();
            var aeonAccount = ConfigurationManager.AppSettings["massAccount"];
            var aeonPassword = ConfigurationManager.AppSettings["massPassword"];
            var baseUrl = ConfigurationManager.AppSettings["massUrl"];
            var url = string.Format("{0}/{1}?id={2}", baseUrl, "Application/get", id);
            var client = ConfigAPI(url, aeonAccount, aeonPassword);
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string httpResponseResult = await response.Content.ReadAsStringAsync();
                    var resultData = JsonConvert.DeserializeObject<ApplicantFromMassDetailByIdViewModel>(httpResponseResult);
                    if (resultData != null)
                    {
                        result.Object = resultData;
                    }
                }

            }
            catch (Exception ex)
            {
                result.ErrorCodes.Add(1004);
                result.Messages.Add("Something went wrong");
                result.Messages.Add(ex.Message);
            }

            return result;

        }
    }
}