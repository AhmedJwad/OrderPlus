using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Enums
{
    public enum OrderType
    {
        [Description("Cash on delivery")]
        PaymentAgainstDelivery,

        [Description("Online payment")]
        PayOnLine
    }
}
