using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entites.Products
{
    public enum StockType
    {
        [EnumMember(Value = "In Stock")]
        INSTOCK,
        [EnumMember(Value = "Low Stock")]
        LOWSTOCK,
        [EnumMember(Value = "Out Of Stock")]
        OUTOFSTOCK,
    }
}
