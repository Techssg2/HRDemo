using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class TradeContractViewModel
    {
        public List<Result> Results { get; set; }
        public class Result
        {
            public DocumentSets DocumentSet { get; set; }
            public string ReferenceNo { get; set; }
            public string SupplierName { get; set; }
            public string SubmittedByLoginName { get; set; }
            public string SubmittedAt { get; set; }
            public string Duedate { get; set; }
        }
        public class DocumentSets
        {
            /*public string DocumentSetId { get; set; }*/
            public string Id { get; set; }
            public string DocumentSetType { get; set; }
            public string DocumentSetPurpose { get; set; }
            public string Description { get; set; }
            /*public Merchandiser Merchandiser { get; set; }*/
            public string ApprovalStatus { get; set; }
            public string Created { get; set; }
        }

        public class ResultDTO
        {
            public object Results { get; set; }
        }
    }
}
