using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Microsoft.Extensions.Configuration;
using AstirOdysseusKos.Web.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using System.Net;
using AstirOdysseusKos.Web.Services;
using System.Text;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Collections.Specialized;
using Umbraco.Cms.Web.Common;
using Our.Umbraco.Honeypot.Core;

namespace AstirOdysseusKos.Web.Controllers
{
    public class formNewsletterController : SurfaceController
    {
        private readonly ILogger<formContactController> _logger;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly UmbracoHelper _umbracoHelper;

        public formNewsletterController(
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
        public async Task<IActionResult> Submit(formNewsletterViewModel model)
        {
            SuccessMessage = "mailerror";
            string retValue = _umbracoHelper.GetDictionaryValue("mailerror");


            if (!ModelState.IsValid && HttpContext.IsHoneypotTrapped())
            {
                return RedirectToCurrentUmbracoPage();
            }
            if (ModelState.IsValid && !HttpContext.IsHoneypotTrapped())
            {

                  using (var client = new WebClient())
                  {
                      var MyValues = new NameValueCollection();
                      MyValues["api_key"] = "1b6ef92ea2aa7d81500f";
                      MyValues["list"] = "Nx7ILwPxx892WmnVCTcctVZA"; 
                      MyValues["boolean"] = "true";
                      MyValues["name"] = model.NameSec;
                      MyValues["email"] = model.EmailSec;
                      MyValues["gdpr"] = "true";

                      var MyResponse = client.UploadValues("https://campaigns.eyewide.gr/subscribe", MyValues);

                      var MyValue = Encoding.Default.GetString(MyResponse);

                      if (MyValue.Equals("true"))
                      {
                          //ola kala
                      }
                      else
                      {
                          // error
                      }
                  }


                StringBuilder builder = new StringBuilder();
                builder.AppendLine("<table>");
                builder.AppendFormat("<tr style='background-color:#f3f3f3'><td><b>{0}</b></b></td><td>{1}</td></tr>", "Name", model.NameSec);
                builder.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "Email", model.EmailSec);
                if (model.AgreeSec)
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
                email.Headers.Add("X-Contact-Form-Type", "newsletter_form");

                email.From.Add(MailboxAddress.Parse(_emailSettings.Value.From));
                foreach (string mailAddress in _emailSettings.Value.mailToNewsletter)
                    email.To.Add(MailboxAddress.Parse(mailAddress));
                email.ReplyTo.Add(MailboxAddress.Parse(model.EmailSec));
                email.Subject = "Newsletter Subscription from website - " + _emailSettings.Value.nameSubject;


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
