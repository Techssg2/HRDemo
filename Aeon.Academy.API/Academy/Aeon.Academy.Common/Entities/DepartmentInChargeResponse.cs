using System;
using System.Collections.Generic;

namespace Aeon.Academy.Common.Entities
{
    public class DepartmentInChargeResponse
    {
        public int Total { get; set; }
        public List<DepartmentInChargeModel> Data { get; set; }
    }
    public class DepartmentInChargeModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string dicCode { get; set; }
        public string code { get; set; }
    }

}
