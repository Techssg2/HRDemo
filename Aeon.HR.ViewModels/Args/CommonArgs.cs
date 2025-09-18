using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels.Args
{
    /* TamPV */
    public class CommonArgs
    {
        /* Is Super Admin*/
        public class Admin
        {
            /* Workflow */
            public class Workflow
            {
                /* Update workflow for Super Admin*/
                public class UpdateWorkflow
                {
                    public Guid WorkflowInstanceId;
                    public Guid? OldDepartmentId { get; set; }
                    public Guid? NewDepartmentId { get; set; }
                }
            }
        }

        public class Member
        {
            public class User
            {
                public class GetAllUserByKeyword
                {
                    public string Predicate { get; set; }
                    public object[] PredicateParameters { get; set; }
                    public string Order { get; set; }
                    public int Page { get; set; }
                    public int Limit { get; set; }

                    public void AddPredicate(string nPredicate, object nPredicateParam)
                    {
                        try
                        {
                            var predicateParams = this.PredicateParameters.ToList();
                            predicateParams.Add(nPredicateParam);
                            this.PredicateParameters = predicateParams.ToArray();
                            nPredicate = nPredicate.Replace("[index]", $"{this.PredicateParameters.Length - 1}");
                            this.Predicate += (this.Predicate.Length > 0 ? " && " : string.Empty) + nPredicate;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                public class GetAllUserByJobgradesList
                {
                    public List<Guid> JobGrades { get; set; }
                    
                }
            }
        }

        public class Department
        {
            public class GetUsersByDeptLines {
            public List<Guid> DepLineIds { get; set; }
            public int Limit { get; set; }
            public int Page { get; set; }
            public string SearchText { get; set; }
            }
        }

        public class SAP
        {
            public class UpdateUserInformationQuoteFromSAP
            {
                public List<string> SapCodes { get; set; }
                public int Year { get; set; }
            }
        }

        public class User
        {
            public class IntergationAPI
            {
                public string KeyWord { get; set; }
                public string Type { get; set; }
                public DateTimeOffset? CreatedFromDate { get; set; }
                public DateTimeOffset? CreatedToDate { get; set; }
                public DateTimeOffset? ModifiedFromDate { get; set; }
                public DateTimeOffset? ModifiedToDate { get; set; }
                public bool? IsActivated { get; set; }

                public string SAPCode { get; set; }
                public string DeptCode { get; set; }
                public string LoginName { get; set; }
                public int Day { get; set; }
                public int Month { get; set; }
                public int Year { get; set; }
                #region DWS
                public DateTime? FromDate { get; set; }
                public DateTime? ToDate { get; set; }
                #endregion
            }
            public class DateValueArgs
            {
                public string Date { get; set; }
                public string Value { get; set; }
            }

            public class ShiftPlanAPIArgs
            {
                public int Day { get; set; }
                public int Month { get; set; }
                public int Year { get; set; }
                public string TimeType { get; set; }
                public string LoginName { get; set; }
                public List<string> ShiftPlan { get; set; }

            }

            public class BTAShifPlanValue
            {
                public string SAPCode { get; set; }
                public DateTimeOffset? FromDate { get; set; }
                public DateTimeOffset? ToDate { get; set; }
                public string DepartureName { get; set; }
                public string ArrivalName { get; set; }
                public string UserName { get; set; }
            }

            public class ActualShiftPlanValue
            {
                public string Date { get; set; }
                public string Value { get; set; }
                public string UserName { get; set; }
            }

            public class ActualBTAValue
            {
                public DateTimeOffset? FromDate { get; set; }
                public DateTimeOffset? ToDate { get; set; }
                public string DepartureName { get; set; }
                public string ArrivalName { get; set; }
                public string UserName { get; set; }
            }
        }
    }
}
