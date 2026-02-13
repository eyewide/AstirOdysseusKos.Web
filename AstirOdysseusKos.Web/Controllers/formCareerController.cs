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
using System.Text;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Our.Umbraco.Honeypot.Core;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Core.Models;



namespace AstirOdysseusKos.Web.Controllers
{
    public class formCareerController : SurfaceController
    {
        private readonly ILogger<formCareerController> _logger;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IOptions<EmailSettings> _emailSettings;
        public formCareerController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            ILogger<formCareerController> logger,
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
            return PartialView("~/Views/Partials/formCareer.cshtml", new formCareerViewModel());
        }
        */
        [TempData]
       public string? SuccessMessage { get; set; }



        [HttpPost]
        [ValidateUmbracoFormRouteString]
        public IActionResult Submit(formCareerViewModel model)
        {
            string retValue = _umbracoHelper.GetDictionaryValue("mailerror");
            SuccessMessage = "mailerror";

            if (!ModelState.IsValid && HttpContext.IsHoneypotTrapped())
            {
                return RedirectToCurrentUmbracoPage();
            }
            if (ModelState.IsValid && !HttpContext.IsHoneypotTrapped())
            {

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("<table>");
                builder.AppendFormat("<tr><td><b>{0}</b></b></td><td>{1}</td></tr>", "Position", model.Position);
                builder.AppendFormat("<tr style='background-color:#f3f3f3'><td><b>{0}</b></b></td><td>{1}</td></tr>", "Name", model.Name);
                builder.AppendFormat("<tr><td><b>{0}</b></b></td><td>{1}</td></tr>", "Surname", model.Surname);
                builder.AppendFormat("<tr style='background-color:#f3f3f3'><td><b>{0}</b></td><td>{1}</td></tr>", "Email", model.Email);
                builder.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "Telephone", model.Telephone);
                builder.AppendFormat("<tr style='background-color:#f3f3f3'><td><b>{0}</b></td><td>{1}</td></tr>", "Message", model.Message);
                builder.AppendFormat("<tr><td><b>{0}</b></td><td>{1}</td></tr>", "I consent to receiving promotional emails", model.ReceiveUpdates);
                builder.AppendFormat("<tr style='background-color:#f3f3f3'><td><b>{0}</b></td><td>{1}</td></tr>", "Agree to the Privacy Policy", model.Agree);
                builder.AppendLine("</table>");

                // create message
                var email = new MimeMessage();
                email.Headers.Add("X-SES-CONFIGURATION-SET", "ContactForms");
                email.Headers.Add("X-Sending-Website", "avdoucollection_gr");
                email.Headers.Add("X-Contact-Form-Type", "career_form");

                email.From.Add(MailboxAddress.Parse(_emailSettings.Value.From));
               // email.To.Add(MailboxAddress.Parse(_emailSettings.Value.mailTo));
                foreach (string mailAddress in _emailSettings.Value.mailToCareer)
                    email.To.Add(MailboxAddress.Parse(mailAddress));
                email.Subject = "Job Application from website - " + _emailSettings.Value.nameSubject;
                var messageBuilder = new BodyBuilder();
                messageBuilder.HtmlBody = string.Format(builder.ToString());
                
                  if (model.attachment != null)
                  {
                      byte[] fileBytes;
                      var file = model.attachment;
                      if (file.Length > 0)
                      {
                          using (var ms = new MemoryStream())
                          {
                              file.CopyTo(ms);
                              fileBytes = ms.ToArray();
                          }
                          messageBuilder.Attachments.Add(file.FileName, fileBytes, MimeKit.ContentType.Parse(file.ContentType));
                      }
                  }
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
            //return Content("");
        }


 
    }
}
