using CheckoutPaymentGateway.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentGateway.Services
{
    public class PoorBankAmbassador : IBankAmbassador
    {
        public bool ProcessPayment(Payment payment)
        {
            // Simple mock up in order to test success and failure case.
            if (payment.Amount > 100)
            {
                return false;
            }

            return true;
        }
    }
}
