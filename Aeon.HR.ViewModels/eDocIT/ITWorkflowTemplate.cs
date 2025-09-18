using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using Aeon.HR.Infrastructure.Abstracts;
using Newtonsoft.Json;

namespace Aeon.HR.ViewModels.eDocIT
{
    public class ITWorkflowTemplate : AuditableEntity
    {
        [StringLength(255)]
        public string WorkflowName { get; set; }
        [StringLength(255)]
        public string ModuleCode { get; set; }
        public string FormTypeCode { get; set; }
        public int Order { get; set; }
        public bool IsActivated { get; set; }
        public bool AllowMultipleFlow { get; set; }
        public string StartWorkflowButton { get; set; }
        public string Note { get; set; }
        public string DefaultCompletedStatus { get; set; }
        public string WorkflowDataStr { get; set; }
        public bool? HasTrackingLog { get; set; }
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