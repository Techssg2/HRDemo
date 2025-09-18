using Aeon.Academy.Common.Workflow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Utils
{
    public static class CommonUtil
    {
        public static string CreateReferenceNumber(string value = "")
        {
            if (string.IsNullOrEmpty(value)) value = DateTime.Now.ToString();
            return $"ATR-{value.GetHashCode().ToString("x").ToUpperInvariant()}-{DateTime.Now.Year}";
        }
        public static WorkflowData DeserializeWorkflow(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            var workflowTemplate = JsonConvert.DeserializeObject<WorkflowData>(data);
            if(workflowTemplate.Steps != null)
                workflowTemplate.Steps = workflowTemplate.Steps.OrderBy(x => x.StepNumber).ToList();

            return workflowTemplate;
        }
        public static string SerializeObject<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }
        public static T DeserializeObject<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
