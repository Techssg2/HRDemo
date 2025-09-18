using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.HR.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RepositoryAttribute : Attribute
    {
        public string Name { get; set; }
        public Type RepositoryType { get; set; }
        public ActionType Action { get; set; }
        public string Role { get; set; }
    }
    public enum ActionType
    {
        None = 0,
        Query = 1,
        Add = 2,
        Update = 4,
        Patch = 8,
        Delete = 16,
        All = 31
    }
}
