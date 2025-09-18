using System.Collections.Generic;

namespace Aeon.Academy.Common.Entities
{
    public class RequestedDepartmentResponse
    {
        public int Total { get; set; }
        public List<RequestedDepartmentModel> Data { get; set; }
    }
    public class RequestedDepartmentModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
