using FluentEmail.Core;
using FluentEmail.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Hsp.Net48.Mail.App
{
    internal class Program
    {
        static string smtp = "smtp.sinocal.com.cn";
        static string from = "tli@sinocal.com.cn";
        static string password = "Tli@691022";
        static string to = "20520578@qq.com";

        static string subject = "Hello world";
        static string body = "Hi, this is Tli. " + DateTime.Now;

        static void Main(string[] args)
        {
            {
                try
                {
                    SmtpClient client = new SmtpClient();
                    client.Host = "smtp.sinocal.com.cn";
                    client.Port = 465;
                    //client.
                    //client.Connect("smtp.sinocal.com.cn", 465, true);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    //client.AuthenticationMechanisms.Remove("XOAUTH2");

                    //// Note: only needed if the SMTP server requires authentication
                    //client.Authenticate("tli@sinocal.com.cn", "Tli@691022");

                    //注意： .NET 4.6 才支持
                    //Email.DefaultSender = new SmtpSender(client);

                    //var email = Email
                    //    .From("tli@sinocal.com.cn")
                    //    .To("20520578@qq.com", "Tli")
                    //    .Subject("星期天去哪里玩？")
                    //    .Body("我想去故宫玩，如何？");

                    //email.Send();

                    //Console.WriteLine("邮件发送成功");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            {
                //var rst = SendMail("Hello World", "Hi, this is Tli.", "20520578@qq.com");

                //if (rst == 1)
                //{
                //    Console.WriteLine("邮件发送成功");
                //}
            }

            {
                //try
                //{
                //    // OK
                //    SendSMTPEMail(smtp, from, password, to, subject, body);
                //    Console.WriteLine("邮件发送成功");
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex);
                //    throw;
                //}
            }

            {



            }

            Console.ReadKey();
        }

        /// <summary>
        /// System.Net.Mail 邮件发送，OK
        /// </summary>
        /// <param name="smtpServer">邮箱服务器</param>
        /// <param name="from">发件人的帐号</param>
        /// <param name="fromPass">发件人密码</param>
        /// <param name="to">收件人帐号</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        public static void SendSMTPEMail(string smtpServer, string from, string fromPass, string to, string subject, string body)
        {
            System.Net.Mail.SmtpClient client = new SmtpClient(smtpServer);
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(from, fromPass);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            System.Net.Mail.MailMessage message = new MailMessage(from, to, subject, body);
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            client.Send(message);
        }



        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="mailBody"></param>
        /// <returns></returns>
        public static int SendMail(string subject, string mailBody, string mailTo)
        {
            SmtpClient client = new SmtpClient("smtp.sinocal.com.cn", 465);
            try
            {
                client.Credentials = new System.Net.NetworkCredential("tli@sinocal.com.cn", "Tli@691022");

                MailMessage message = new MailMessage();

                message.From = new MailAddress("tli@sinocal.com.cn", "tli", System.Text.Encoding.UTF8);

                //string[] mailTos = ConfigurationManager.AppSettings["Mailto"].ToString().Split(';');

                foreach (string mailToTest in mailTo.Split(';'))
                {
                    if (!string.IsNullOrEmpty(mailToTest))
                        message.To.Add(new MailAddress(mailToTest));
                }

                message.Body = mailBody;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                client.EnableSsl = true;
                client.TargetName = "STARTTLS/smtp.sinocal.com.cn";

                client.Send(message);
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.Message + "  " + ex.StackTrace);
                return 0;
            }
            finally
            {
                client.Dispose();
            }
        }








    }
}
