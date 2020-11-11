using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using ProjectManagementSystem.Common;
using ProjectManagementSystem.Resources;

namespace ProjectManagementSystem.Common
{
    public class SentMailAuto
    {
        protected static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int SentMail(string email, string param, string userAccount)
        {
            int sucess = 0;
            try
            {
                var msg = new MailMessage();
                string url = HttpContext.Current.Request.Url.ToString();
                var action = string.Empty;
                if (url.IndexOf("PasswordReissue") != -1)
                {
                    action = HttpContext.Current.Request.Url.ToString().Replace("PasswordReissue", "SetNewPassword?infor=" + param);
                }
                else
                {
                    action = HttpContext.Current.Request.Url.ToString().Replace("InputCompanyCode", "SetNewPassword?infor=" + param);
                }
                 
                msg.From = new MailAddress(ConfigurationManager.AppSettings[ConfigurationKeys.SMTP_USER], "i-PRo");
                msg.To.Add(email);
                msg.Subject = Resources.Messages.I002;
                string link = "<a href='" + action + "'> <i><u>Click here</u></i> </a>";
                msg.Body = String.Format(Resources.Messages.I003, link, userAccount, ConfigurationManager.AppSettings[ConfigurationKeys.SMTP_SUPPORT]);
                msg.IsBodyHtml = true;
                using (var client = new SmtpClient())
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings[ConfigurationKeys.SMTP_USER], ConfigurationManager.AppSettings[ConfigurationKeys.SMTP_PASS]);
                    client.Host = ConfigurationManager.AppSettings[ConfigurationKeys.SMTP_SERVER];
                    client.Port = Convert.ToInt32(ConfigurationManager.AppSettings[ConfigurationKeys.SMTP_PORT]);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(msg);
                    msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                    sucess = 1;
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                sucess = 0;
                logger.Error(ex);
            }
            return sucess;
        }
    }
}