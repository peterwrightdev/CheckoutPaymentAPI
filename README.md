# CheckoutPaymentAPI
## API for performing Payment actions for Checkout Ltd

This RESTful API is a development example of a Payment Gateway to provide the following requirements;

1. A merchant should be able to process a payment through the payment gateway and receive either a
successful or unsuccessful response
2. A merchant should be able to retrieve the details of a previously made payment

## The API exposes the following actions;

### POST request action;
CheckoutPaymentGateway/PaymentReports/MakePayment

Example URL when running debug build;
https://localhost:5001/CheckoutPaymentGateway/PaymentReports/MakePayment

No headers expected or required.

Requires body containing representation of a Payment to be processed. Raw JSON body example like so;

{
        "CardNumber": "4539798211688974",
        "ExpiryDate": "2019-01-06T17:16:40",
        "Amount": 101.000,
        "CurrencyCode": "GBP",
        "CardVerificationValue": "123"
}

Some initial validation is carried out on the request by use of attributes. Error will be returned to user if CardNumber is obviously not a valid card number, or the CardVerificationValue(CVV) is not 3 digits in length. This pre-validation could be improved upon in future updates to encompass a larger set of obvious user error cases to quickly error invalid requests, or removed entirely if product intent would be to rely soley on the validation of the acquiring bank.

Once intial validation is carried out, API will process the Payment with the Acquiring Bank. In the current implementation, this is mocked out with a service which will return a failure if the "Amount" is greater than 100, or success otherwise. By use of service injection, this service could be replaced with a proper implementation to handle communication with the bank, such as calling an API on the bank's side.

A PaymentReport is then created based on the Payment's details, the time of the request and the response from the "Bank". This PaymentReport is intented to function as our accounting records of attempted payments. This PaymentReport is stored in an in-memory database and then returned to the caller in the body of the response to their request. An example PaymentReport in JSON format is like so;

{
    "id": 1,
    "wasSuccessful": false,
    "paymentDateTime": "2020-06-06T15:04:25.2838935Z",
    "amount": 101.000,
    "CurrencyCode": "GBP",
    "maskedCardNumber": "453979******8974",
    "expiryDate": "2019-01-06T17:16:40",
    "cardVerificationValue": "123"
}

id is a unique identified for the record created, currently an auto-incrementing integer. An improvement would be to use a GUID.

wasSuccesful indicates whether the Bank accepted the payment. In this case, the Bank rejected the Payment as the Amount was over 100.

paymentDateTime indicates the DateTime of when the request was processed in UTC.

maskedCardNumber is the CardNumber supplied in the request, with only the first 6 and last 4 digits kept.

The remaining fields are all directly populated from the Payment record. A note on this is that the regulations on storing and sending card details have not been fully implemented at this time.

### GET request action
CheckoutPaymentGateway/PaymentReports
CheckoutPaymentGateway/PaymentReports/1

Example URL when running debug build;
https://localhost:5001/CheckoutPaymentGateway/PaymentReports
https://localhost:5001/CheckoutPaymentGateway/PaymentReports/1

No headers expected or required.

Returns list all of PaymentReports stored on the in-memory database, or in the case of a single id, one single record.
Records will be returned like so;

{
    "id": 1,
    "wasSuccessful": false,
    "paymentDateTime": "2020-06-06T15:04:25.2838935Z",
    "amount": 101.000,
    "CurrencyCode": "GBP",
    "maskedCardNumber": "453979******8974",
    "expiryDate": "2019-01-06T17:16:40",
    "cardVerificationValue": "123"
}

As database is currently an in-memory database, these records are not persistent should the application be stopped. This would be improved by implementing a data storage system such as SQL.

## Additional source code notes
A few simple unit tests have been included in the project to secure basic functionality. These should be extended to cover a wider range of tests given more time. These use injection to mock out services and test logic in isolation.
