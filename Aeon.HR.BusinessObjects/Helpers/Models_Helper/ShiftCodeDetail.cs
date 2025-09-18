using Aeon.HR.BusinessObjects.Helpers;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Models
{
    public class ShiftCodeDetail
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameVN { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }


        public ShiftCodeDetail(MasterExternalDataViewModel shiftCode_masterData)
        {
            if(null != shiftCode_masterData)
            {
                this.Code = shiftCode_masterData.Code;
                this.Name = shiftCode_masterData.Name;
                this.NameVN = shiftCode_masterData.NameVN;
                this.StartTime = shiftCode_masterData.StartTime;
                this.EndTime = shiftCode_masterData.EndTime;
            }
        }

        public DateTime GetStartDateTime(DateTime otDate)
        {
            DateTime returnValue = DateTime.MinValue;
            try
            {
                string dateString = otDate.ToString("yyyy-MM-dd");
                dateString += " "+ StartTime;
                returnValue = DateTime.ParseExact(dateString, "yyyy-MM-dd HHmmss", null);
            }
            catch
            {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }

        public DateTime GetEndDateTime(DateTime otDate)
        {
            DateTime returnValue = DateTime.MinValue;
            try
            {
                string dateString = otDate.ToString("yyyy-MM-dd");
                dateString += " " + EndTime;
                returnValue = DateTime.ParseExact(dateString, "yyyy-MM-dd HHmmss", null);
            }
            catch
            {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }

    }
    public class ShiftCodeDetailCollection:List<ShiftCodeDetail>
    {
        public ShiftCodeDetailCollection()
        {

        }
        public ShiftCodeDetailCollection(List<MasterExternalDataViewModel> shiftCodeCollection_masterData)
        {
            if(null != shiftCodeCollection_masterData)
            {
                this.AddRange(shiftCodeCollection_masterData.Select(x=>new ShiftCodeDetail(x)));
            }
        }

        public ShiftCodeDetail GetDetailsByCode(string shiftCode)
        {
            ShiftCodeDetail returnValue = null;
            try
            {
                returnValue = this.FirstOrDefault(x => x.Code.Equals(shiftCode, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
            }
            return returnValue;
        }
    }
}
