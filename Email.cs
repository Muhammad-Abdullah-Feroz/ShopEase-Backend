using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace ShopEase_Backend
{
    public class Email
    {
        static void main(string[] args)
        {
            try
            {
                MailMessage newMail = new MailMessage();
                // use the Gmail SMTP Host
                SmtpClient client = new SmtpClient("smtp.gmail.com");

                // Follow the RFS 5321 Email Standard
                newMail.From = new MailAddress("SENDER-EMAIL", "SENDER-NAME");

                newMail.To.Add("<<RECIPIENT-EMAIL>>");// declare the email subject

                newMail.Subject = "Account verification"; // use HTML for the email body

                newMail.IsBodyHtml = true; newMail.Body = "<h1> Your verification code is </h1>";

                // enable SSL for encryption across channels
                client.EnableSsl = true;
                // Port 465 for SSL communication
                client.Port = 465;
                // Provide authentication information with Gmail SMTP server to authenticate your sender account
                client.Credentials = new System.Net.NetworkCredential("<<SENDER-EMAIL>>", "<<SENDER-GMAIL-PASSWORD>>");

                client.Send(newMail); // Send the constructed mail
                Console.WriteLine("Email Sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error -" + ex);
            }
        }
    }
}