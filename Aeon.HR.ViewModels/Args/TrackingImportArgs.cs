using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels.Args
{
    public class TrackingImportArgs
    {
        public string Predicate { get; set; }
        public object[] PredicateParameters { get; set; }
        public string Order { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public object Object { get; set; }
        public String referenceId { get; set; }
        public String referenceNumber { get; set; }
        public List<String> referenceStatus { get; set; }

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
            catch
            {

            }
        }
    }
}