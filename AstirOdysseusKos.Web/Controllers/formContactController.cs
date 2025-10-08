using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using AstirOdysseusKos.Web.Models;
using System.Text;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Website.Controllers;
using Our.Umbraco.Honeypot.Core;
using MailKit.Net.Smtp;

namespace AstirOdysseusKos.Web.Controllers
{
    public class formContactController : SurfaceController
    {
        private readonly ILogger<formContactController> _logger;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly UmbracoHelper _umbracoHelper;

        public formContactController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            ILogger<formContactController> logger,
             UmbracoHelper umbracoHelper,
            IOptions<EmailSettings> emailSettings)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _emailSettings = emailSettings;
            _umbracoHelper = umbracoHelper;
            _logger = logger;
        }
        /*
        public IActionResult Index()
        {
            return PartialView("~/Views/Partials/formContact.cshtml", new formContactViewModel());
        }
        */
        [TempData]
        public string? SuccessMessage { get; set; }


        [HttpPost]
        [ValidateUmbracoFormRouteString]
        public async Task<IActionResult> Submit(formContactViewModel model)
        {
            SuccessMessage = "mailerror";
            string retValue = _umbracoHelper.GetDictionaryValue("mailerror");

            if (!ModelState.IsValid && HttpContext.IsHoneypotTrapped())
            {
                return RedirectToCurrentUmbracoPage();
            }
            if (ModelState.IsValid && !HttpContext.IsHoneypotTrapped())
            {

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("<table>");

                builder.AppendFormat("<tr style='background-color:#f3f3f3'><td><b>{0}</b></td><td>{1}</td></tr>", "Name", model.Name);
                builder.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "Surname", model.Surname);
                builder.AppendFormat("<tr style='background-color:#f3f3f3'><td><b>{0}</b></td><td>{1}</td></tr>", "Email", model.Email);
                builder.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "Telephone", model.Telephone);
                builder.AppendFormat("<tr style='background-color:#f3f3f3'><td><b>{0}</b></td><td>{1}</td></tr>", "Message", model.Message);

                if (model.ReceiveUpdates)
                {
                    builder.AppendFormat("<tr style='background-color:#f3f3f3'><td><b>{0}</b></td><td>{1}</td></tr>", "I consent to receiving promotional emails", "yes");
                }
                else
                {
                    builder.AppendFormat("<tr style='background-color:#f3f3f3'><td><b>{0}</b></td><td>{1}</td></tr>", "I consent to receiving promotional emails", "no");
                }
                if (model.Agree)
                {
                    builder.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "Agree to the Privacy Policy", "yes");
                }
                else
                {
                    builder.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "Agree to the Privacy Policy", "no");
                }

                builder.AppendLine("</table>");


                // create message
                var email = new MimeMessage();
                email.Headers.Add("X-SES-CONFIGURATION-SET", "ContactForms");
                email.Headers.Add("X-Sending-Website", "astirodysseuskos_com");
                email.Headers.Add("X-Contact-Form-Type", "contact_form");

                email.From.Add(MailboxAddress.Parse(_emailSettings.Value.From));
                foreach (string mailAddress in _emailSettings.Value.mailTo)
                    email.To.Add(MailboxAddress.Parse(mailAddress));
                email.ReplyTo.Add(MailboxAddress.Parse(model.Email));
                email.Subject = "Contact from website - " + _emailSettings.Value.nameSubject;


                var messageBuilder = new BodyBuilder();
                messageBuilder.HtmlBody = string.Format(builder.ToString());

                email.Body = messageBuilder.ToMessageBody();

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect(_emailSettings.Value.Host, _emailSettings.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailSettings.Value.Username, _emailSettings.Value.Password);
                try
                {
                    smtp.Send(email);
                    smtp.Disconnect(true);

                    SuccessMessage = "mailsent";
                    retValue = _umbracoHelper.GetDictionaryValue("mailsent");

                    // return Json(new { success = true, message = retValue });
                    return Content(retValue);

                }
                catch (Exception ex)
                {
                    //throw;
                    _logger.LogError(ex.Message, ex);
                    return Content(retValue);
                }


            }

            return RedirectToCurrentUmbracoPage();
        }



    }
}
