using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderProcessing.Lib
{
    public class ShipmentNotice
    {
        #region Tools
        private const string CommentPreamble = "Please include the following comments to our customer: ";
        private static readonly Regex rxOrdernumber = new Regex(@"Order (\d+) \((.*)\).*""(.+)""", RegexOptions.Compiled);
        private static readonly Regex rxCurrency = new Regex(@"\$(\d+(\.\d+)?)", RegexOptions.Compiled);
        private static readonly string[] delimiters = new string[] { "\r\n" };
        #endregion
        #region Methods
        public static ShipmentNotice FromEmail(MailMessage email)
        {
            //Order (\d+) \((.*)\).*"(.+)"
            if (email is null)
                throw new ArgumentNullException(nameof(email));
            else if (email.Subject != "Order Shipped")
                throw new ArgumentNullException($"Wrong email type: {email.Subject}");
            if (rxCurrency.Matches(email.Body).Count < 5)
                throw new ArgumentException("Too few currency symbols found.");
            // Match the body of the email to Regex expressing
            Match match = rxOrdernumber.Match(email.Body);

            if (!match.Success)
                throw new ArgumentException("Invalid email body");

            ShipmentNotice notice = new ShipmentNotice
            {
                OrderNumber = int.Parse(match.Groups[1].Value),
                CustomerName = match.Groups[2].Value,
                EmailTo = match.Groups[3].Value
            };
            string[] lines = email.Body.Split(delimiters, StringSplitOptions.None);

            for (int nLine = 0; nLine < lines.Length; ++nLine)
            {
                if (lines[nLine] == notice.CustomerName)
                {
                    List<string> addressLines = new List<string>();
                    // Skips each line until next return
                    addressLines.AddRange(lines.Skip(nLine + 1).TakeWhile(l => l != ""));
                    notice.AddressLines = addressLines.AsReadOnly();
                    continue;
                }
                if (lines[nLine].StartsWith("Item Description"))
                {
                    // Convert strings into order details
                    List<OrderDetail> details = lines.Skip(nLine + 1).
                        TakeWhile(l => l != "").
                        Select(l => new OrderDetail(l)).
                        ToList();
                    notice.OrderDetails = details.AsReadOnly();
                    continue;
                }
                if (lines[nLine].StartsWith("The tax"))
                {
                    MatchCollection matches = rxCurrency.Matches(lines[nLine]);
                    notice.Tax = decimal.Parse(matches[0].Value.Substring(1));
                    notice.Shipping = decimal.Parse(matches[1].Value.Substring(1));
                    notice.OrderTotal = decimal.Parse(matches[2].Value.Substring(1));
                    continue;
                }
                if(lines[nLine].StartsWith(CommentPreamble))
                {
                    notice.CustomerComments = lines[nLine].
                        Substring(CommentPreamble.Length).
                        Replace("\"", "");
                    break;
                }
            }
            return notice;
        }
        #endregion
        #region Classes
        public int OrderNumber { get; private set; }
        public string CustomerName { get; private set; }
        public string EmailTo { get; private set; }
        public IReadOnlyCollection<string> AddressLines { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Shipping { get; private set; }
        public decimal OrderTotal { get; private set; }
        public string CustomerComments { get; private set; }
        public IReadOnlyCollection<OrderDetail> OrderDetails { get; private set; }
        #endregion
    }
}
