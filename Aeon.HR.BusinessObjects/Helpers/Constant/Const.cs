using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects
{
    public static class Const
    {
        // Danh sach can update workflow CR 9.6
        public static List<string> FieldNameGrades = new List<string>() { "NewJobGradeName", "MaxGrade", "RequestedJobGrade", "JobGradeCaption" };
        public static List<string> FieldNameGradesIT = new List<string>() { "NewJobGradeName", "MaxGrade", "RequestedJobGrade", "JobGradeCaption" };
        public static List<string> FieldNameGradesFacility = new List<string>() { "NewJobGradeName", "MaxGrade", "RequestedJobGrade", "JobGradeCaption" };
        public struct Status
        {
            public const string draft = "Draft";
            public const string completed = "Completed";
            public const string rejected = "Rejected";
            public const string requestedToChange = "Requested To Change";
            public const string cancelled = "Cancelled";
            public const string outOfPeriod = "Out Of Period";
        }

        public struct SAPStatus
        {
            public const string FAIL = "FAIL";
            public const string SUCCESS = "SUCCESS";
        }

        public struct SAPErrorLog
        {
            public const string DATA_IS_ALREADY_EXISTED = "DATA IS ALREADY EXISTED";
        }
    }
}
