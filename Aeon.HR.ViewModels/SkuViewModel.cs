using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class SkuViewModel
    {
        public int StatusCode { get; set; }
        public string StatusText { get; set; }
        public string Message { get; set; }
        public DataOb Data { get; set; }
        public class DataOb
        {
            public int Count { get; set; }
            public int Skip { get; set; }
            public int Take { get; set; }
            public List<ResultOb> Result { get; set; }

            public class ResultOb
            {
                public string ID { get; set; }
                public string ReferenceNumber { get; set; }
                public string Created { get; set; }
                public string Modified { get; set; }
                public string CreatedByFullName { get; set; }
                public string ModifiedByFullName { get; set; }
                public string SAPCode { get; set; }
                public string UserId { get; set; }
                public string DepartmentId { get; set; }
                public string Description { get; set; }
                public string CreatedById { get; set; }
                public string ModifiedById { get; set; }
                public string CreatedBy { get; set; }
                public string ModifiedBy { get; set; }
                public string DepartmentName { get; set; }
                public string DepartmentCode { get; set; }
                public string UserName { get; set; }
                public string Link { get; set; }
                public string Status { get; set; }
                public string ItemType { get; set; }
            }
        }


    }
}
