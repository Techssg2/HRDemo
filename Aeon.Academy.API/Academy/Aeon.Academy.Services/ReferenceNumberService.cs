using Aeon.Academy.Data;
using Aeon.Academy.Data.Entities;
using Aeon.Academy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aeon.Academy.Services
{
    class ReferenceNumberService
    {
        private readonly IGenericRepository<ReferenceNumber> repository = null;
        
        public ReferenceNumberService(IUnitOfWork<EDocDbContext> eDocUnitOfWork)
        {
            this.repository = eDocUnitOfWork.GetRepository<ReferenceNumber>();
        }

        public string GenerateReferenceNumber(string name)
        {
            var refNumber = string.Empty;
            //get ReferenceNumber
            var referenceNumber = repository.Query(q => q.ModuleType == name).FirstOrDefault();
            if (referenceNumber.IsNewYearReset && referenceNumber.CurrentYear != DateTime.Now.Year || referenceNumber.CurrentYear == 0)
            {
                referenceNumber.CurrentNumber = 1;
                referenceNumber.CurrentYear = DateTime.Now.Year;
            }
            else
            {
                referenceNumber.CurrentNumber++;
            }
            //Update
            repository.Update(referenceNumber);
            refNumber = referenceNumber.Formula;
            var tokens = FindFieldTokens(refNumber);
            foreach (var token in tokens)
            {
                switch (token.ToLower())
                {
                    case "{year}":
                        refNumber = refNumber.Replace(token, referenceNumber.CurrentYear.ToString());
                        break;
                    //For Autonumber field
                    default:
                        var tokenParts = token.Trim(new char[] { '{', '}' }).Split(new char[] { ':' });
                        if (tokenParts.Length > 1)
                        {
                            refNumber = refNumber.Replace(token, referenceNumber.CurrentNumber.ToString($"D{tokenParts[1]}"));
                        }
                        break;
                }
            }
            return refNumber;
        }
        private IEnumerable<string> FindTokens(string str, string pattern)
        {
            var regex = new Regex(pattern);
            var matches = regex.Matches(str);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    yield return match.Value;
                }
            }
        }
        private IEnumerable<string> FindFieldTokens(string str)
        {
            var tokens = FindTokens(str, @"\{[\d\w\s\:]*\}");
            foreach (var token in tokens)
            {
                yield return token;
            }
        }
    }
}
