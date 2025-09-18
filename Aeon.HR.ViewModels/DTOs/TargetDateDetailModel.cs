using Aeon.HR.Infrastructure.Abstracts;
using Aeon.HR.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.ViewModels
{
    public class TargetDateDetail
    {
        public string date { get; set; }
        //Shift Code
        public string value { get; set; }
    }
    public class TargetDateDetailCollection : List<TargetDateDetail>
    {
        public TargetDateDetailCollection()
        {

        }
        public TargetDateDetail GetByDate(DateTime date)
        {
            TargetDateDetail returnValue = null;
            try
            {
                returnValue = this.FirstOrDefault(x => x.date != null && x.date.Equals(date.ToString("yyyyMMdd")));
            }
            catch
            {
            }
            return returnValue;
        }
    }
}