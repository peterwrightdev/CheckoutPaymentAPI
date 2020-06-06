using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CheckoutPaymentGateway.DTOs;

namespace CheckoutPaymentGateway.DataContexts
{
    public class PaymentsContext : DbContext
    {
        public PaymentsContext()
            : base()
        {
        }

        public PaymentsContext(DbContextOptions<PaymentsContext> options)
            : base(options)
        {
        }

        public DbSet<PaymentReport> PaymentReports;

        public DbSet<CheckoutPaymentGateway.DTOs.PaymentReport> PaymentReport { get; set; }
    }
}
