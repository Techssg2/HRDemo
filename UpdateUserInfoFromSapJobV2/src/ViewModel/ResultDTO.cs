using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateUserInfoFromSapJobV2.src.ViewModel
{
    public class ResultDTO
    {
        public ResultDTO()
        {
            ErrorCodes = new List<int>();
            ErrorCodeStr = new List<string>();
            Messages = new List<string>();
            Descriptions = new List<string>();
        }
        public ResultDTO(object value)
        {
            Object = value;
        }
        public List<int> ErrorCodes { get; set; }
        public List<string> ErrorCodeStr { get; set; }
        public List<string> Descriptions { get; set; }
        public List<string> Messages { get; set; }
        public bool CheckLock { get; set; }
        public object Object { get; set; }
        public bool IsSuccess => !ErrorCodes.Any();
        public void ResetResult()
        {
            ErrorCodes = new List<int>();
            Messages = new List<string>();
        }
    }
    public class ArrayResultDTO
    {
        public ArrayResultDTO()
        {
            Data = new object { };
            Count = 0;
        }
        public int Count { get; set; }
        public object Data { get; set; }
    }
}
