using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Helpers
{
    public static class ListUtilities
    {
        // HR-908 Điều chỉnh rule Workflow của Participant type "HR Department" lấy đến Dept paren G6 của các phòng ban
        /*  SM BUSINESS(G6) -(dept code: 50026662)
            GLAM BEAUTIQUE(G6) -(Dept code: 50030106)
            MERCHANDISING STRATEGY &TACTICS(G6) - (Dept code: 50025719) */
        public static readonly List<string> notInDept = new List<string>() { "50030106", "50025719", "50026662" };

    }
}
