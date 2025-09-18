using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TargetPlanTesting.ImportData;
using System.Linq;

namespace Aeon.HR.ViewModels
{
    public class ImportTrackingViewModel
    {
        public string Module { get; set; }
        public string FileName { get; set; }
        public string Documents { get; set; }
        public string JsonDataStr { get; set; }
        public string Status { get; set; }

        /*public string Status
        {
            get
            {
                if (!string.IsNullOrEmpty(JsonDataStr) && !string.IsNullOrEmpty(Module))
                {
                    try
                    {
                        string returnValue = "";
                        switch (Module)
                        {
                            case "RequestToHire":
                                returnValue = JsonConvert.DeserializeObject<List<ImportRequestToHireError>>(JsonDataStr).Any(x => !string.IsNullOrEmpty(x.Status) && x.Status.Equals("Failure")) ? "Failure" : "Success";
                                break;
                        }
                        return returnValue;
                    }
                    catch (Exception)
                    {
                        return ""; // ImportRequestToHireError
                    }
                }
                else
                    return "";
            }
        }*/
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid? CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
    }
}
