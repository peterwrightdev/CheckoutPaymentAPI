using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentGateway.Services
{
    public class DateTimeService
    {
        public virtual DateTime UtcDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}
