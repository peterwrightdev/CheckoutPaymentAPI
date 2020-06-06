using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CheckoutPaymentGateway.DataContexts;
using CheckoutPaymentGateway.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheckoutPaymentGateway.DTOs;

namespace CheckoutPaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentReportsController : ControllerBase
    {
        private PaymentsContext _context;

        private IBankAmbassador _bankAmbassador;

        private readonly ILogger<PaymentReportsController> _logger;

        // Use dependency injection to allow mocking of datetime used at runtime.
        private DateTimeService _dateTimeService;

        public PaymentReportsController(ILogger<PaymentReportsController> logger, PaymentsContext paymentsContext, DateTimeService dateTimeService, IBankAmbassador bankAmbassador)
        {
            this._logger = logger;
            this._context = paymentsContext;
            this._dateTimeService = dateTimeService;
            this._bankAmbassador = bankAmbassador;
        }

        // GET: api/PaymentReports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentReport>>> GetPaymentReport()
        {
            return await _context.PaymentReport.ToListAsync();
        }

        // GET: api/PaymentReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentReport>> GetPaymentReport(int id)
        {
            var paymentReport = await _context.PaymentReport.FindAsync(id);

            if (paymentReport == null)
            {
                return NotFound();
            }

            return paymentReport;
        }

        // POST: api/PaymentReports/MakePayment
        [HttpPost]
        [Route("MakePayment")]
        public async Task<ActionResult<PaymentReport>> MakePayment(Payment payment)
        {
            // Populate the report of the payment based on Payment details.
            PaymentReport report = new PaymentReport()
            {
                Amount = payment.Amount,
                CardVerificationValue = payment.CardVerificationValue,
                ExpiryDate = payment.ExpiryDate,
                MaskedCardNumber = this.MaskCardNumber(payment.CardNumber),
                PaymentDateTime = this._dateTimeService.UtcDateTime(),
                WasSuccessful = this._bankAmbassador.ProcessPayment(payment),
            };

            // Add PaymentReport to the database and return the created report to the caller.
            this._context.Add(report);
            await this._context.SaveChangesAsync();

            return CreatedAtAction("GetPaymentReport", new { id = report.Id }, report);
        }

        // This method could belong to a lower level LogicContext class as and when logic is needed in multiple controllers.
        private string MaskCardNumber(string unmaskedCardNumber)
        {
            if (unmaskedCardNumber.Length >= 10)
            {
                return unmaskedCardNumber.Substring(0, 6) + new string('*', unmaskedCardNumber.Length - 10) + unmaskedCardNumber.Substring(unmaskedCardNumber.Length - 4, 4);
            }
            else
            {
                return new string('*', unmaskedCardNumber.Length);
            }
        }
    }
}
