using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using Aeon.HR.Infrastructure.Abstracts;
using Newtonsoft.Json;

namespace Aeon.HR.ViewModels.eDocIT
{
    public class ITWorkflowInstance
    {
        public Guid Id { get; set; }
        public string WorkflowName { get; set; }
        public string DefaultCompletedStatus { get; set; }
        public Guid ITTemplateId { get; set; }
        public string ITWorkflowDataStr { get; set; }
        public Guid ItemId { get; set; }
        public string ItemReferenceNumber { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsTerminated { get; set; }
        public string ItemStatus { get; set; }
        public string StepConditions { get; set; }
        
        public bool ParseData(DataRow dr)
        {
            try
            {
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    string _colName = dr.Table.Columns[i].ColumnName;
                    PropertyInfo _prop = this.GetType().GetProperty(_colName);
                    if (!(dr[_colName] is DBNull) && _prop != null)
                    {
                        _prop.SetValue(this, dr[_colName], null);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}