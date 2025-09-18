using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum FieldType
    {
        String = 0,
        Int = 1,
        Float = 2,
        Double = 3,
        Date = 4,
        Guid = 5,
        Boolean = 6,
        Enum = 7,
        BooleanAsInt = 8,
        DateTimeOfSet = 9
    }
}