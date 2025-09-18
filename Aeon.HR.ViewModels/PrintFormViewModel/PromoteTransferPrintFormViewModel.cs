using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.PrintFormViewModel
{
    public class PromoteTransferPrintFormViewModel
    {
        public string FullName { get; set; }
        public string UserSAPCode { get; set; }
        public string CurrentPositionName { get; set; }
        public string CurrentDepartmentName { get; set; }
        public string CurrentJobGradeName { get; set; }
        public string CurrentWorkLocationName { get; set; }
        //CR222====================================================
        public string PersonnelArea { get; set; }
        public string PersonnelAreaText { get; set; }
        public string EmployeeGroup { get; set; }
        public string EmployeeGroupDescription { get; set; }
        public string EmployeeSubgroup { get; set; }
        public string EmployeeSubgroupDescription { get; set; }
        public string PayScaleArea { get; set; }
        //========================================================
        public string NewPositionName { get; set; }
        public string NewDepartmentName { get; set; }
        public string NewJobGradeName { get; set; }
        public Guid? NewJobGradeId { get; set; }
        public string NewWorkLocationName { get; set; }
        public string NewSalaryBenefit { get; set; }
        public string ReportToUser { get; set; }
        public string EffectiveDate { get; set; }
        public string RequestFrom { get; set; }
        public string RequestFromDes { get; set; }
        public string ReasonOfPromotion { get; set; }
        public bool IsStoreNewDepartment { get; set; }
        public bool IsSameDepartment { get; set; }
        // Addition
        public string LineManager { get; set; } // Step1
        public string LManagerSignedDate { get; set; } // Step1
        public string StoreManager { get; set; } // Step2
        public string SManagerSignedDate { get; set; } // Step2
        public string SGMOperation { get; set; } // Step3
        public string SGMOperationSignedDate { get; set; } // Step3
        public string NewLineManager { get; set; } // Step4
        public string NewLManagerSignedDate { get; set; } // Step4 
        public string NewStoreManager { get; set; } // Step5
        public string NewSManagerSignedDate { get; set; } // Step5
        public string NewSGMOperation { get; set; } // Step6
        public string NewSGMOperationSignedDate { get; set; } // Step6 
        public string HRManager { get; set; }
        public string HRManagerSignedDate { get; set; }
        public string GeneralDirector { get; set; }
        public string GDirectorSignedDate { get; set; }

    }
}
