using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

using EasyPoll.Models;

namespace EasyPoll
{
    public static class MailSender
    {
        public static void SendEmails(List<UserModel> recievers, string message)
        {
            recievers.Where(r => r.Email != null).ToList().ForEach(receiver =>
            {
                var senderEmail = new MailAddress("noreply.easypoll@gmail.com", "EasyPoll");
                var receiverEmail = new MailAddress(receiver.Email, receiver.Username);
                var password = "pwd123123";
                var body = message;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = "Опрос",
                    Body = body
                })
                {
                    smtp.Send(mess);
                }
            });
        }
    }
}
