using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace orion.web.Common
{
    public interface ISmtpProxy : IRegisterByConvention
    {
        void SendMail(string recipient, string body, string subject);
    }

    public class SmtpProxy : ISmtpProxy
    {
        private readonly IConfiguration config;
        private readonly ILogger<SmtpProxy> logger;

        public SmtpProxy(IConfiguration config, ILogger<SmtpProxy> logger)
        {
            this.config = config;
            this.logger = logger;
        }

        public void SendMail(string recipient, string body, string subject)
        {
            try
            {
                var emailUser = config.GetValue<string>("EmailUserName");
                var fromAddress = new MailAddress(emailUser, "no-reply@orion-us.com");                
                var toAddress = new MailAddress(recipient);
                var fromPassword = config.GetValue<string>("EmailPassword");
                var smtp = new SmtpClient
                {
                    Host = config.GetValue<string>("EmailHost"),
                    Port = config.GetValue<int>("EmailPort"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception while trying to send notification email");
                throw;
            }
        }
    }
}
