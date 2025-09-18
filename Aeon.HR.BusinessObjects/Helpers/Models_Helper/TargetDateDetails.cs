using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Models
{
    public class TargetDateDetail
    {
        public string date { get; set; }
        //Shift Code
        public string value { get; set; }
    }
    public class TargetDateDetailCollection:List<TargetDateDetail>
    {
        public TargetDateDetailCollection()
        {

        }
        public TargetDateDetail GetByDate(DateTime date)
        {
            TargetDateDetail returnValue = null;
            try
            {
                returnValue = this.FirstOrDefault(x=>x.date != null && x.date.Equals(date.ToString("yyyyMMdd")));
            }
            catch
            {
            }
            return returnValue;
        }
    }
}
