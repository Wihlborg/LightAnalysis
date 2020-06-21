using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MailKit;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace UserAuthentication
{
    class Mail
    {

        public void send(string reciever, string password)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("LightAnalysis", "droidtjack@gmail.com"));
            mailMessage.To.Add(new MailboxAddress("User", reciever));
            mailMessage.Subject = "New Password";
            mailMessage.Body = new TextPart("plain")
            {
                Text = password
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");

                smtpClient.Authenticate("droidtjack@gmail.com", "hkrtjack127");
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }

    }
}
