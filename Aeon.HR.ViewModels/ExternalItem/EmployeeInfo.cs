using Aeon.HR.Infrastructure.Attributes;
using Aeon.HR.Infrastructure.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Aeon.HR.ViewModels.ExternalItem
{
    [MyClass(APIName = "Add_EmployeeSet")]
    public class EmployeeInfo: ISAPEntity
    {
        public EmployeeInfo()
        {
            ApplicantRelativeInAeons = new HashSet<FamilyMemberInfo>();
            ApplicantEducations = new HashSet<EducationInfo>();
            EmergencyContacts = new HashSet<EmergencyContactInfo>();
            AddressInfomations = new HashSet<AddressInfo>();
            IDCardInfos = new HashSet<IDCardInfo>();
        }
        public string EdocId { get; set; } // Applicant ID
        public string EdocUser { get; set; } // Current User
        public string EmployeeGroupCode { get; set; } 
        public string ReasonHiringCode { get; set; }
        public string ActionType { get; set; } // = "Z1"
        public string EmployeeSybGroupCode { get; set; }
        public string FirstName { get; set; } // Thi Ngoc Bich
        public string LastName { get; set; } // Tran
        public string Title { get; set; }
        public string WorkLocation { get; set; } // Location Code
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PlaceOfBirth { get; set; }
        public string CityCode { get; set; }
        public string Ethnic { get; set; } // Mass ko co
        public string Religion { get; set; } // Mass ko co
        public string Nationality { get; set; }
        public string SecondNationanlity { get; set; }        
        public string Marital { get; set; }
        public string PersonnelArea { get; set; }
        public string EmailAdress { get; set; } // email ca nhan
        public string ComEmailAdress { get; set; }     // email company      
        public string StartDate { get; set; }
        public string SapCode { get; set; }
        public string UpdateEmployee { get; set; }
        [JsonProperty("List_FamilyMemberSet")]
        public ICollection<FamilyMemberInfo> ApplicantRelativeInAeons { get; set; }
        [JsonProperty("List_EducationSet")]
        public ICollection<EducationInfo> ApplicantEducations { get; set; }
        [JsonProperty("List_EmergencyContactSet")]
        public ICollection<EmergencyContactInfo> EmergencyContacts { get; set; }
        [JsonProperty("List_AddressSet")]
        public ICollection<AddressInfo> AddressInfomations { get; set; }
        [JsonProperty("List_IcnumSet")]
        public ICollection<IDCardInfo> IDCardInfos { get; set; }
    }
    public class FamilyMemberInfo
    {
        public string FamilyMemberRelationShip { get; set; }
        public string FamilyMemberFirstName { get; set; }
        public string FamilyMemberLastName { get; set; }
        public string FamilyMemberGender { get; set; }
    }
    public class EducationInfo
    {
        public string EducationLevel { get; set; }
        public string EducationStartDate { get; set; }
        public string EducationEndDate { get; set; }
        public string EducationCertificate { get; set; }
        public string EducationSchoolName { get; set; }

    }
    public class EmergencyContactInfo
    {
        public string EmergencyContactFirstName { get; set; }
        public string EmergencyContactLastName { get; set; }
        public string EmergencyContactRelationship { get; set; }
    }
    public class AddressInfo
    {
        public string AddressType { get; set; }
        public string Address { get; set; }
        public string WardCode { get; set; }
        public string DistrictCode { get; set; }
        public string ProvinceCode { get; set; }
        public string CountryCode { get; set; }
    }
    public class IDCardInfo
    {
        public string IDCardType { get; set; }
        public string IDCardNo { get; set; }
        public string IDCardDate { get; set; }
        public string IDCardPlace { get; set; }
    }
}