using Aeon.HR.BusinessObjects.DataHandlers;
using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.Data.Models;
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
using Aeon.HR.Data;
using Newtonsoft.Json;
using System.Reflection;
using Aeon.HR.ViewModels;
using Aeon.HR.Infrastructure.Enums;

namespace Aeon.HR.BusinessObjects
{
    #region Department
    public static class DepartmentHelper
    {
        public static Department GetParentDepartment(this Department dept)
        {
            Department returnValue = null;
            try
            {
                returnValue = dept.Parent;
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static List<Guid> GetAllParentDepartmentID(this Department dept, int JobGradeTo = 9)
        {
            List<Guid> returnValue = new List<Guid>();
            try
            {
                if (!(dept is null) && dept.JobGrade.Grade <= JobGradeTo)
                {
                    returnValue.Add(dept.Id);
                    if (dept.ParentId != Guid.Empty)
                    {
                        returnValue.AddRange(dept.Parent.GetAllParentDepartmentID(JobGradeTo));
                    }
                }
            }
            catch
            {
                returnValue = new List<Guid>();
            }
            return returnValue;
        }
        public static List<Department> GetAllChilDepartment(IUnitOfWork uow, Department dept)
        {
            List<Department> returnValue = new List<Department>();
            try
            {
                returnValue = uow.GetRepository<Department>(true).FindBy(x => x.ParentId == dept.Id).ToList();
            }
            catch
            {
                returnValue = new List<Department>();
            }
            return returnValue;
        }
        public static List<string> GetAllChilDepartmentCode(this Department dept, IUnitOfWork uow, int JobGradeTo = 1)
        {
            List<string> returnValue = new List<string>();
            try
            {

                if (!(dept is null) && dept.JobGrade.Grade >= JobGradeTo)
                {
                    returnValue.Add(dept.Code);
                    var allChilDepts = GetAllChilDepartment(uow, dept);

                    if (allChilDepts.Count > 0)
                    {
						foreach (var currentDept in allChilDepts)
						{
                            returnValue.AddRange(currentDept.GetAllChilDepartmentCode(uow));
                        }
                    }
                }
            }
            catch
            {
                returnValue = new List<string>();
            }
            return returnValue;
        }

        public static List<Department> GetAll_HRDepartment(this IUnitOfWork uow, bool isStore = false, int JobGradeFrom = 1, int JobGradeTo = 9)
        {
            List<Department> returnValue = new List<Department>();
            try
            {
                #region Local variables
                Func<int, bool> validateJobGrade = (jobGradeValue) =>
                {
                    return jobGradeValue >= 1 && jobGradeValue <= 9;
                };
                JobGradeFrom = validateJobGrade(JobGradeFrom) ? JobGradeFrom : 1;
                JobGradeTo = validateJobGrade(JobGradeTo) ? JobGradeTo : 9;
                #endregion

                returnValue = uow.GetRepository<Department>(true).FindBy(dept => dept != null && dept.IsHR
                    && !dept.IsDeleted
                    && dept.IsStore == isStore
                    && dept.JobGrade.Grade >= JobGradeFrom
                    && dept.JobGrade.Grade <= JobGradeTo
                ).OrderBy(x => x.JobGrade.Grade).ToList();
            }
            catch
            {
            }
            return returnValue;
        }


        public static Department GetHRDepartment(this Department dept, IUnitOfWork uow, string JobGradeFrom = "", string JobGradeTo = "")
        {
            Department returnValue = null;
            try
            {
                int jgMin = JobGradeFrom.Replace("G", string.Empty).GetAsInt();
                int jgMax = JobGradeTo.Replace("G", string.Empty).GetAsInt();
                returnValue = dept.GetHRDepartment(uow, jgMin, jgMax);
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
        public static Department GetHRDepartment(this Department dept, IUnitOfWork uow, int JobGradeFrom = 1, int JobGradeTo = 9)
        {
            Department returnValue = null;
            try
            {
                if (!(uow is null))
                {
                    JobGradeFrom = JobGradeFrom <= 0 ? 1 : JobGradeFrom;
                    JobGradeTo = JobGradeTo <= 0 ? 9 : JobGradeTo;
                    //HR manager: Just only HQ has HR manager
                    List<Department> hrDepts = uow.GetAll_HRDepartment(false, JobGradeFrom, JobGradeTo);
                    if (!(hrDepts is null))
                    {
                        List<Guid> allParent_Of_CurrentDept = dept.GetAllParentDepartmentID();
                        returnValue = hrDepts.FirstOrDefault(x => x != null && x.GetAllParentDepartmentID().Intersect(allParent_Of_CurrentDept).Count() > 0);
                    }
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
        public static Department GetHRManagerDepartment(this Department dept, IUnitOfWork uow, int JobGradeFrom = 1, int JobGradeTo = 9)
        {
            Department returnValue = null;
            try
            {
                Department hrDepartment = dept.GetHRDepartment(uow, JobGradeFrom, JobGradeTo);
                if (!(hrDepartment is null))
                {
                    returnValue = hrDepartment.Parent;
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }

        public static Department GetRootDepartment(this Department department)
        {
            Department returnValue = department;
            try
            {
                if (returnValue != null)
                {
                    while (returnValue.JobGrade.Grade < 5)
                    { 
                        returnValue = returnValue.Parent;
                    }
                }
            }
            catch
            {
                returnValue = null;
            }
            return returnValue;
        }
    }
    #endregion
}
