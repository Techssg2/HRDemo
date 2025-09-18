using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.Academy.Common.Entities
{
    public class EdocTask<T>
    {
        public EdocTask()
        {

            Edoc1Tasks = new List<T>();
            Edoc2Tasks = new List<T>();
        }
        public NotificationUser User { get; set; }
        public NotificationUser CCUser { get; set; }
        public List<T> Edoc1Tasks { get; set; } // Edoc2
        public List<T> Edoc2Tasks { get; set; }
    }
}
