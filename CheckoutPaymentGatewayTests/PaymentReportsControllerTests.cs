using System;
using NUnit.Framework;
using CheckoutPaymentGateway.Controllers;
using CheckoutPaymentGateway.DTOs;
using Microsoft.Extensions.Logging;
using Moq;
using CheckoutPaymentGateway.DataContexts;
using CheckoutPaymentGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutPaymentGatewayTests
{
    public class PaymentReportsControllerTests
    {
        private Mock<ILogger<PaymentReportsController>> _mockLogger;
        private Mock<PaymentsContext> _mockContext = new Mock<PaymentsContext>();
        private Mock<DateTimeService> _mockDateTimeService = new Mock<DateTimeService>();
        private Mock<IBankAmbassador> _mockBankAmbassador = new Mock<IBankAmbassador>();

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<PaymentReportsController>>();
            _mockContext = new Mock<PaymentsContext>();
            _mockDateTimeService = new Mock<DateTimeService>();
            _mockBankAmbassador = new Mock<IBankAmbassador>();
        }

        [Test]
        public void SuccessTestCase()
        {
            _mockDateTimeService.Setup(dts => dts.UtcDateTime()).Returns(new DateTime(2020, 01, 01));
            Payment testPayment = new Payment()
            {
                Amount = 15,
                CardNumber = "4539798211688974",
                CardVerificationValue = "123",
                CurrencyCode = "GBP",
                ExpiryDate = new DateTime(2020, 01, 02),
            };

            _mockBankAmbassador.Setup(ba => ba.ProcessPayment(testPayment)).Returns(true);

            PaymentReportsController testController = new PaymentReportsController(_mockLogger.Object, _mockContext.Object, _mockDateTimeService.Object, _mockBankAmbassador.Object);
            var result = testController.MakePayment(testPayment).GetAwaiter().GetResult();

            CreatedAtActionResult castResult = result.Result as CreatedAtActionResult;
            Assert.AreEqual(testPayment.Amount, ((PaymentReport)castResult.Value).Amount);
            Assert.AreEqual(testPayment.CardVerificationValue, ((PaymentReport)castResult.Value).CardVerificationValue);
            Assert.AreEqual("453979******8974", ((PaymentReport)castResult.Value).MaskedCardNumber);
            Assert.AreEqual(new DateTime(2020, 01, 01), ((PaymentReport)castResult.Value).PaymentDateTime);
            Assert.AreEqual(new DateTime(2020, 01, 02), ((PaymentReport)castResult.Value).ExpiryDate);
            Assert.AreEqual(true, ((PaymentReport)castResult.Value).WasSuccessful);
        }

        [Test]
        public void FailureTestCase()
        {
            _mockDateTimeService.Setup(dts => dts.UtcDateTime()).Returns(new DateTime(2020, 01, 01));
            Payment testPayment = new Payment()
            {
                Amount = 15,
                CardNumber = "4539798211688974",
                CardVerificationValue = "123",
                CurrencyCode = "GBP",
                ExpiryDate = new DateTime(2020, 01, 02),
            };

            // Mock BankAmbassador service returns false, so Bank rejected the payment.
            _mockBankAmbassador.Setup(ba => ba.ProcessPayment(testPayment)).Returns(false);

            PaymentReportsController testController = new PaymentReportsController(_mockLogger.Object, _mockContext.Object, _mockDateTimeService.Object, _mockBankAmbassador.Object);
            var result = testController.MakePayment(testPayment).GetAwaiter().GetResult();

            CreatedAtActionResult castResult = result.Result as CreatedAtActionResult;
            Assert.AreEqual(testPayment.Amount, ((PaymentReport)castResult.Value).Amount);
            Assert.AreEqual(testPayment.CardVerificationValue, ((PaymentReport)castResult.Value).CardVerificationValue);
            Assert.AreEqual("453979******8974", ((PaymentReport)castResult.Value).MaskedCardNumber);
            Assert.AreEqual(new DateTime(2020, 01, 01), ((PaymentReport)castResult.Value).PaymentDateTime);
            Assert.AreEqual(new DateTime(2020, 01, 02), ((PaymentReport)castResult.Value).ExpiryDate);

            // Same results as success case except the PaymentReport created indicates failure.
            Assert.AreEqual(false, ((PaymentReport)castResult.Value).WasSuccessful);
        }
    }
}