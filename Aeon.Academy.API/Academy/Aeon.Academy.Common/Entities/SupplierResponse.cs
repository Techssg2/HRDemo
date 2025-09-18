using System.Collections.Generic;

namespace Aeon.Academy.Common.Entities
{
    public class SupplierResponse
    {
        public int Total { get; set; }
        public List<SupplierModel> Data { get; set; }
    }
    public class SupplierModel
    {
        public string SAPName { get; set; }
        public string SAPCode { get; set; }
    }
}
