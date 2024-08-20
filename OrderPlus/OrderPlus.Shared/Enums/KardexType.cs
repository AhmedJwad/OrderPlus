using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.Enums
{
    public enum KardexType
    {
        [Description("Purchase")]
        Purchase,

        [Description("Order")]
        Order,

        [Description("CancelOrder")]
        CancelOrder,

        [Description("Inventory")]
        Inventory
    }
}
