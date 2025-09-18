using Aeon.HR.Data.Models;
using Aeon.HR.ViewModels.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.ViewModels
{
    public class RevokeBTAInfoViewModel
    {
        public string ChangeCancelBusinessTripDetailStr { get; set; }
        public Guid BusinessTripApplicationId { get; set; }
        //public BusinessTripApplication BTA { get; set; }
        public List<ChangeCancelBusinessTripDTO> ChangeCancelBusinessTripDetails
        {
            get
            {
                List<ChangeCancelBusinessTripDTO> returnValue = new List<ChangeCancelBusinessTripDTO>();
                if (!string.IsNullOrEmpty(ChangeCancelBusinessTripDetailStr))
                {
                    returnValue = JsonConvert.DeserializeObject<List<ChangeCancelBusinessTripDTO>>(ChangeCancelBusinessTripDetailStr);
                }
                return returnValue;
            }
            set
            {
                if (value is null || value.GetType() != typeof(PricedItinerariesViewModel))
                {
                    ChangeCancelBusinessTripDetailStr = string.Empty;
                }
                else
                {
                    ChangeCancelBusinessTripDetailStr = JsonConvert.SerializeObject(value);
                }
            }
        }
    }
}
