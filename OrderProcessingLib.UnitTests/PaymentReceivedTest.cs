using System;
using System.IO;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OrderProcessing.Lib.UnitTests
{
    [TestClass]
    public class PaymentReceivedTest
    {
        [TestMethod]
        public void FromEmail()
        {
            string emailBody = File.ReadAllText(@"C:\Users\perscholas_student\Desktop\uipath\CaseStudy\EmailProcessing\SendMail\PaymentReceived\Payment Received - Pericles Comidas clásicas.txt");
            MailMessage email = new MailMessage
            {
                Subject = "Payment Received",
                Body = emailBody
            };
            PaymentReceived notice = PaymentReceived.FromEmail(email);
        }
    }
}