using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Enums
{
    public enum OrderStatus
    {
        [Description("New")]
        New,

        [Description("Shipped")]
        Dispatched,

        [Description("Sent")]
        Sent,

        [Description("Confirmed")]
        Confirmed,

        [Description("Cancelled")]
        Cancelled
    }
}
