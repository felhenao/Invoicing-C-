using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SendMail
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Endter O (Order Request), p (Payment Received), S (Shipment Notices), R (Restock Notices)");
            string input = Console.ReadLine().ToUpper();
            string folder = null, subject = null;
            switch(input) {
                 case "O":
                    folder = "OrderRequests";
                    subject = "Order Request";
                    break;
                case "P":
                    folder = "PaymentReceived";
                    subject = "Payment Received";
                    break;
                case "S":
                    folder = "ShipmentNotices";
                    subject = "Order Shipped";
                    break;
                case "R":
                    folder = "RestockNotices";
                    subject = "Shipment Received";
                    break;
                default: Console.WriteLine("Nothing to do! Bye."); return;
            }
            foreach (string fpath in Directory.GetFiles(folder))
                SendMessage(fpath, subject);
            {

            }
        }
        static void SendMessage(string fileName, string subject)
        {
            string body = File.ReadAllText(fileName);
            MailMessage msg = new MailMessage("nwadmin@platform.com",
                "rpa16@platformbronx.com", subject, body);
            SmtpClient client = new SmtpClient("10.0.0.35");
            client.Send(msg);
        }
    }
}
