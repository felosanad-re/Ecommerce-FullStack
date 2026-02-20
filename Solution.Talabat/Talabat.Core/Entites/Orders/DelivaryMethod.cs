using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entites.Orders
{
    public class DelivaryMethod : ModelBase
    {
        public DelivaryMethod()
        {
            
        }
        public DelivaryMethod(string shortName, string description, string delivaryTime, decimal cost)
        {
            ShortName = shortName;
            Description = description;
            DelivaryTime = delivaryTime;
            Cost = cost;
        }

        public string ShortName { get; set; }
        public string Description { get; set; }
        public string DelivaryTime { get; set; }
        public decimal Cost { get; set; }
    }
}
