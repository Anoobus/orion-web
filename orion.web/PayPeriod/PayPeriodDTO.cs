using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Common
{
    public class PayPeriodListDTO
    {
        public List<PayPeriodDTO> PayPeriodList { get; set; }
        public PayPeriodDTO SelectedPayPeriod { get; set; }
    }

    public class PayPeriodDTO
    {
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public int StartWeekId { get; set; }
        public int EndWeekId { get; set; }        
    }
}
