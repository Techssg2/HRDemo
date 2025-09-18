using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.HR.Infrastructure
{
    public class Result<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Count { get; set; }
        public string ErrorMessage { get; set; }
        public Result(IEnumerable<T> item, int count)
        {
            Items = item;
            Count = count;
        }
    }
    public class SingleResult<T>
    {
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public SingleResult(T data)
        {
            Data = data;
        }
    }
    public class SavingResult<T>
    {
        public T Model { get; set; }
        public object Result { get; set; }
        public string ErrorMessage { get; set; }
        public SavingResult(T model, object result)
        {
            Model = model;
            Result = result;
        }
    }
}
