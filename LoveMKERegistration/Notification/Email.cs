using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace LoveMKERegistration.Notification
{
    public static class Email
    {
        public static bool Send(MailMessage myMessage)
        {
            string smtp = ConfigurationManager.AppSettings["Email:smtp"];
            string username = ConfigurationManager.AppSettings["Email:Email"];
            string password = ConfigurationManager.AppSettings["Email:Password"];
            myMessage.From = new MailAddress(username);
            myMessage.Bcc.Add(new MailAddress(username));


            try
            {
                SmtpClient client = new SmtpClient(smtp);
                client.Credentials = new NetworkCredential(username, password);
                client.EnableSsl = true;
                client.Port = 587;
                client.Send(myMessage);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public static string GetConfirmationRegistration()
        {
            string confirmation = "Thank you for signing up to be a part of LoveMKE!  Through our intentional acts of service, we hope people connect and grow with God. Your project leader will be in touch with you regarding the project(s) you signed up for.  If you have any questions, please contact the church office- 414.427.1929.";
            return confirmation;
        }
        public static string GetWebsiteDetails()
        {
            string website = "\n\nFor more details visit: www.southbrookministries.org\n\n";
            return website;
        }
    }
}