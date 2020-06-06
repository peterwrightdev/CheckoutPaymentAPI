using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CheckoutPaymentGateway.DTOs
{
    public class PaymentReport
    {
        [Key]
        public int Id { get; set; }

        public bool WasSuccessful { get; set; }

        public DateTime PaymentDateTime { get; set; }

        public decimal Amount { get; set; }

        // TODO; verify PCI compliance on this. Looks like can keep first 6 digits and last 4. Rest blanked out.
        public string MaskedCardNumber { get; set; }

        // Not sure if these two should be returned or even stored.
        // This example app probably doesn't need to be PCI compliant. Including them for now
        public DateTime ExpiryDate { get; set; }

        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV must be 3 digits.")]
        public string CardVerificationValue { get; set; }
    }
}
