using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace Institute.WebApi.Controllers
{
    public class NotificationMail
    {
        public static void SendMail(List<string> ToEmails, string Name, string Type)
        {
            string Pass, FromEmailid, HostAdd;

            //Reading sender Email credential from web.config file
            HostAdd = ConfigurationManager.AppSettings["Host"].ToString();
            FromEmailid = ConfigurationManager.AppSettings["FromMail"].ToString();
            Pass = ConfigurationManager.AppSettings["Password"].ToString();

            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(FromEmailid); 
            mailMessage.Subject = "Pliant Notification";
            mailMessage.Body = Name +" "+ Type + " Added"; 
            mailMessage.IsBodyHtml = true;

            foreach (var EmailId in ToEmails)
            {
                mailMessage.To.Add(new MailAddress(EmailId)); //adding multiple TO Email Id
            }
         
            SmtpClient smtp = new SmtpClient(); 
            smtp.Host = HostAdd; 

            //network and security related credentials
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential();
            NetworkCred.UserName = mailMessage.From.Address;
            NetworkCred.Password = Pass;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            smtp.Send(mailMessage); 
        }
    }
}