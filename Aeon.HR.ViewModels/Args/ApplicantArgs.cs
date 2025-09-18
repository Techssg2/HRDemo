using Aeon.HR.Data;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class ApplicantArgs: AuditableEntity
    {
        public ApplicantArgs()
        {
            ApplicantRelativeInAeons = new HashSet<ApplicantRelativeInAeon>();
            ApplicantEducations = new HashSet<ApplicantEducation>();
            ApplicantWorkingProcesses = new HashSet<ApplicantWorkingProcess>();
            Activities = new HashSet<Activity>();
            CharacterReferences = new HashSet<CharacterReferee>();
            EmploymentHistories = new HashSet<EmploymentHistory>();
            ApplicantRelativeInAeons = new HashSet<ApplicantRelativeInAeon>();           
            FamilyMembers = new HashSet<FamilyMember>();
            EmergencyContacts = new HashSet<EmergencyContact>();
            InterviewEvaluates = new HashSet<InterviewEvaluate>();
            LanguageProficiencyEntries = new HashSet<LanguageProficiencyEntry>();

        }
        public Guid Id { get; set; }
        public Guid? ApplicantStatusId { get; set; }
        public Guid? AppreciationId { get; set; }
        public Guid? PositionId { get; set; }
        public ApplicantType Type { get; set; }
        public string Position1Name { get; set; }
        public string Position2Name { get; set; }
        public float? GrossSalary1 { get; set; }
        public int? TerminationNotice1 { get; set; }
        public float? GrossSalary2 { get; set; }
        public int? TerminationNotice2 { get; set; }
        public string SAPCode { get; set; }
        public string ReferenceNumber { get; set; }
        public string FullName { get; set; }
        public string GenderCode { get; set; }
        public string GenderName { get; set; }
        public string NationalityCode { get; set; }
        public string NationalityName { get; set; }
        public string Mobile { get; set; }
        public bool? HasEmailAddress { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string NativeCode { get; set; }
        public string NativeName { get; set; }
        public string BirthPlaceCode { get; set; }
        public string BirthPlaceName { get; set; }
        public bool? HaveIdentifyCardNumber12 { get; set; }
        // ID Card 9
        public string IDCard9Number { get; set; }
        public DateTimeOffset? IDCard9Date { get; set; }
        public string IDCard9PlaceCode { get; set; }
        public string IDCard9PlaceName { get; set; }
        // ID Card 12
        public string IDCard12Number { get; set; }
        public DateTimeOffset? IDCard12Date { get; set; }
        public string IDCard12PlaceCode { get; set; }
        public string IDCard12PlaceName { get; set; }
        // Passport
        public string PassportNo { get; set; }       
        public string PassportType { get; set; }
        // Permanent Resident
        public string PermanentResidentAddress { get; set; }
        public string PermanentResidentCityCode { get; set; }
        public string PermanentResidentCityName { get; set; }
        public string PermanentResidentDistrictCode { get; set; }
        public string PermanentResidentDistrictName { get; set; }
        public string PermanentResidentWardCode { get; set; }
        public string PermanentResidentWardName { get; set; }

        // Provisional Resident
        public string ProvisionalResidentAddress { get; set; }
        public string ProvisionalResidentCityCode { get; set; }
        public string ProvisionalResidentCityName { get; set; }
        public string ProvisionalResidentDistrictCode { get; set; }
        public string ProvisionalResidentDistrictName { get; set; }
        public string ProvisionalResidentWardCode { get; set; }
        public string ProvisionalResidentWardName { get; set; }
        public bool? HasConvictedForCrime { get; set; }
        public string ConvictedForCrimeNotes { get; set; }
        public bool? UsedToInterviewInCompany { get; set; }
        public string InterviewedForVacancyInCompanyNotes { get; set; }
        public bool? HasHealthProblem { get; set; }
        public string InformationAboutHealthProblem { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public bool? HasPregnant { get; set; }
        public bool? HaveChildrenUnder12Month { get; set; }
        public bool? HaveRelativeInAeon { get; set; }
        public Guid? UserId { get; set; }

        public string EducationLevelCode { get; set; }
        public string EducationLevelName { get; set; }

        public string ApplicantAppliedPosition1 { get; set; }
        public string ApplicantAppliedPosition2 { get; set; }
        public string ApplicantAppliedPosition3 { get; set; }

        public Guid? WorkingTimeRecruitmentId { get; set; }
        public string AppropriateTimeCode { get; set; }
        public string AppropriateTimeName { get; set; }
        public bool? HasExperienceOnWorkingInShift { get; set; }
        public bool? HasExperienceOnStandingAndWalking { get; set; }
        public bool? HasExperienceOnRetail { get; set; }
        public string ExpectedStartingDateCode { get; set; }
        public string ExpectedStartingDateName { get; set; }
        public DateTimeOffset? ExpectedStartingDateOther { get; set; }
        public string FullTimeWorkingExprerimentCode { get; set; }
        public string FullTimeWorkingExprerimentName { get; set; }
        public bool? CanWorkEarlyOrLateShift { get; set; }
        public string SkillsCode { get; set; }
        public string SkillsName { get; set; }
        public string AbilitiesCode { get; set; }
        public string AbilitiesName { get; set; }
        public string LanguagesCode { get; set; }
        public string LanguagesName { get; set; }

        public string InterestsInWork1stPriorityCode { get; set; }
        public string InterestsInWork1stPriorityName { get; set; }
        public string InterestsInWork2ndPriorityCode { get; set; }
        public string InterestsInWork2ndPriorityName { get; set; }
        public string InterestsInWork3rdPriorityCode { get; set; }
        public string InterestsInWork3rdPriorityName { get; set; }
        public string InterestsInWork4thPriorityCode { get; set; }
        public string InterestsInWork4thPriorityName { get; set; }
        public string InterestsInWork5thPriorityCode { get; set; }
        public string InterestsInWork5thPriorityName { get; set; }
        public string InterestsInWork6thPriorityCode { get; set; }
        public string InterestsInWork6thPriorityName { get; set; }
        public string InterestsInWork7thPriorityCode { get; set; }
        public string InterestsInWork7thPriorityName { get; set; }
        public string InterestsInWork8thPriorityCode { get; set; }
        public string InterestsInWork8thPriorityName { get; set; }
        public string InterestsInWork9thPriorityCode { get; set; }
        public string InterestsInWork9thPriorityName { get; set; }
        public string InterestsInWork10thPriorityCode { get; set; }
        public string InterestsInWork10thPriorityName { get; set; }
        public bool? HaveAppliedInAeon { get; set; }
        public bool? HavePension { get; set; }
        public string HaveAppliedInAeonDetail { get; set; }
        public bool? HaveWorkedInAeon { get; set; }
        public string ExpectedSalary { get; set; }

        public string KnowJobFromWeb { get; set; }
        public string KnowJobFromSchool { get; set; }
        public string KnowJobFromOthers { get; set; }

        public DateTimeOffset? StartDate { get; set; }
        public string EmployeeGroupCode { get; set; }
        public string EmployeeSybGroupCode { get; set; }
        public string ReasonHiringCode { get; set; }
        
        public bool? HavePassport { get; set; }
        public string ReligionCode { get; set; }
        public string ReligionName { get; set; }
        public string Passport { get; set; }
        public DateTimeOffset PassportDate { get; set; }
        public string PassportPlaceCode { get; set; }
        public string PassportPlaceName { get; set; }        
        public BloodGroup? BloodGroup { get; set; }
        public bool? DrivingsLicense { get; set; }
        public bool? PossessOwnVehicle { get; set; }
        public string PossessOwnVehicleType { get; set; }
        public bool? HasMotorbike { get; set; }   // Có sở hữu phương tiện đi lại? Xe may
        public bool? HasCar { get; set; }   // Có sở hữu phương tiện đi lại? Xe hoi
        public string CarNo { get; set; }
        public string MotocycleNo { get; set; }      //end
        public string EmergencyContactNumber { get; set; }
        public string EmergencyContactPerson { get; set; }
        public string EmergencyContactPersonRelationship { get; set; }// Quan hê của Người có thể liên hệ trong trường hợp khẩn cấp
        public string EmergencyContactPersonRelationshipName { get; set; }// Quan hê của Người có thể liên hệ trong trường hợp khẩn cấp
        public string PresentHobbies { get; set; }
        public string ComputerSkills { get; set; }
        public string TypingSpeed { get; set; }
        public string OtherSkills { get; set; }
        public bool HasCreateF2Form { get; set; }
        public string InChargePerson { get; set; }
        public string SAPReviewStatus { get; set; }
        public  ICollection<Activity> Activities { get; set; }       
        public  ICollection<ApplicantRelativeInAeon> ApplicantRelativeInAeons { get; set; }
        public  ICollection<ApplicantEducation> ApplicantEducations { get; set; }
        public  ICollection<ApplicantWorkingProcess> ApplicantWorkingProcesses { get; set; }
        public ICollection<CharacterReferee> CharacterReferences { get; set; }
        public ICollection<EmploymentHistory> EmploymentHistories { get; set; }
        public  ICollection<EmergencyContact> EmergencyContacts { get; set; }
        public  ICollection<FamilyMember> FamilyMembers { get; set; }
        public  ICollection<InterviewEvaluate> InterviewEvaluates { get; set; }
        public ICollection<LanguageProficiencyEntry> LanguageProficiencyEntries { get; set; }
       

    }
}