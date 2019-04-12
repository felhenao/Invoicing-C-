using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderProcessing.Lib
{
    public class PaymentReceived
    {
        private static readonly Regex rxAmount = new Regex(@"\$(\d+\,?\d+\.?\d+)", RegexOptions.Compiled);
        private static readonly Regex rxOrderNum = new Regex(@"order\s(\d+)", RegexOptions.Compiled);
        private static readonly Regex rxDate = new Regex(@"on\s(\d+\-\d+\-\d+)", RegexOptions.Compiled);
        private static readonly Regex rxCheckNum = new Regex(@"is\s(\d+)", RegexOptions.Compiled);

        public static PaymentReceived FromEmail(MailMessage email)
        {
            Match matchAmount = rxAmount.Match(email.Body);
            Match matchOrderNum = rxOrderNum.Match(email.Body);
            Match matchDate = rxDate.Match(email.Body);
            Match matchCheckNum = rxCheckNum.Match(email.Body);
            if (email is null) throw new ArgumentNullException(nameof(email));
            if (email.Subject != "Payment Received")
                throw new ArgumentException($"Wrong email type:{email.Subject}");
            if (!matchCheckNum.Success) throw new ArgumentException("Invalid email body");

            PaymentReceived notice = new PaymentReceived
            {
                Amount = decimal.Parse(matchAmount.Groups[1].Value),
                OrderNum = int.Parse(matchOrderNum.Groups[1].Value),
                Date = DateTime.Parse(matchDate.Groups[1].Value),
                CheckNum = int.Parse(matchCheckNum.Groups[1].Value),
            };
            return notice;
        }
        public int CheckNum { get; private set; }
        public DateTime Date { get; private set; }
        public int OrderNum { get; private set; }
        public decimal Amount { get; private set; }
    }
}
