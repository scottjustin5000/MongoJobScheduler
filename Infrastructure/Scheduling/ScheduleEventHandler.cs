using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Scheduling
{
    /// <summary>
    /// Provides the event handler for schedules.
    /// </summary>
    public delegate void ScheduleEventHandler(object sender, ScheduleEventArgs e);
}
