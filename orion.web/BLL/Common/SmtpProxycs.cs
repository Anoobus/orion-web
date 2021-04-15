using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using orion.web.Util;
using orion.web.Util.IoC;
using System;
using System.Net;
using System.Net.Mail;

namespace orion.web.Common
{
    public interface ISmtpProxy
    {
        void SendMail(string recipient, string body, string subject);
    }

    public class SmtpProxy : ISmtpProxy, IAutoRegisterAsSingleton
    {
        private readonly IConfiguration config;
        private readonly ILogger<SmtpProxy> logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SmtpProxy(IConfiguration config, ILogger<SmtpProxy> logger, IHostingEnvironment hostingEnvironment)
        {
            this.config = config;
            this.logger = logger;
            _hostingEnvironment = hostingEnvironment;
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
                using(var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                })
                {
                    if(_hostingEnvironment.EnvironmentName == "Development")
                    {
                        var debug = new
                        {
                            fromAddress,
                            toAddress,
                            subject,
                            body
                        };
                        logger.LogInformation("Email Sent: " + debug.Dump());
                    }
                    else
                    {
                        //smtp.Send(message);
                    }
                }
            }
            catch(Exception e)
            {
                logger.LogError(e, "Exception while trying to send notification email");
                throw;
            }
        }
    }
}
