using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using FakeSmtp.Helpers;
using FakeSmtp.Models;
using netDumbster.smtp;

namespace FakeSmtp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static SimpleSmtpServer SmtpServer { get; set; }

        public static bool IsSmtpServerOn { get; set; }

        public static int MaximumLimit { get; set; } = 1000;

        public static FixedSizeAndReversedOrderQueue<Email> Inbox { get; set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Inbox = new FixedSizeAndReversedOrderQueue<Email>(MaximumLimit);

            StartSmtpServer(5000, MaximumLimit);
        }

        protected void Application_End()
        {
            SmtpServer.Stop();
            IsSmtpServerOn = false;
        }

        public static void StartSmtpServer(int port, int limit)
        {
            Inbox = new FixedSizeAndReversedOrderQueue<Email>(limit, Inbox);

            SmtpServer = SimpleSmtpServer.Start(port);
            IsSmtpServerOn = true;
            MaximumLimit = limit;

            SmtpServer.MessageReceived += SmtpServer_MessageReceived;
        }

        public static void StopSmtpServer()
        {
            SmtpServer.MessageReceived -= SmtpServer_MessageReceived;
            SmtpServer.ClearReceivedEmail();
            SmtpServer.Stop();
            IsSmtpServerOn = false;
        }

        private static void SmtpServer_MessageReceived(object sender, MessageReceivedArgs e)
        {
            var newEmailId = Inbox.Any() ? Inbox.First().Id + 1 : 1;

            Inbox.Insert(new Email(e.Message, newEmailId));

            SmtpServer.ClearReceivedEmail();
        }
    }
}