#region Using Directives
using System.Text.RegularExpressions;
using System.Web;
using AstirOdysseusKos.Tools.Client;
using Eyewide.Tools.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
#endregion
namespace AstirOdysseusKos.Tools.NotificationHandler;
public class ImportBlogPosts : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
{
  #region Fields
  private readonly ILogger<ImportBlogPosts> _logger;
  private readonly IRuntimeState _runtimeState;
  private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
  private IContentService _contentService;
  private readonly Guid _blogRoot;
  private readonly IBlogService _blogService;
  #endregion

  #region Constructor
  public ImportBlogPosts(ILogger<ImportBlogPosts> logger, IRuntimeState runtimeState, Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment, IContentService contentService, IJsonSerializer jsonSerializer, IUploadPostImage uploadPostImage, IBlogService blogService)
  {
    _logger = logger;
    _runtimeState = runtimeState;
    _hostingEnvironment = hostingEnvironment;
    _contentService = contentService;
    _blogService = blogService;
    _blogRoot = Guid.Parse("e42aed92-a2dd-4491-ad34-f7856861d5e3");
  }
  #endregion

  #region Main Method
  public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
  {

    if (_runtimeState.Level < RuntimeLevel.Run)
    {
      _logger.LogInformation("Umbraco is not in Run level. Blog post import skipped.");
      return;
    }
    _logger.LogInformation("Starting blog post import...");
    await ImportPostsAsync();
  }
  #endregion

  #region Import Posts Method
  private async Task ImportPostsAsync()
  {
    _logger.LogInformation("Importing blog posts...");
    try
    {
      // Fetch blog posts from external service
      // For each blog post, create content nodes in Umbraco
      var blogPosts = await _blogService.GetAllBlogPostsAsync(count: 81, skip: 150);
      foreach (var post in blogPosts)
      {
        _logger.LogInformation($"Processing blog post: {post.Title}");
        var postFeaturedImage = post.ImageToUpload;
        var simpleExcerpt = !string.IsNullOrEmpty(post.Excerpt) ? Regex.Replace(post.Excerpt, "<.*?>", String.Empty) : "";
        var cleanTitle = !string.IsNullOrEmpty(post.Title) ? HttpUtility.HtmlDecode(post.Title) : "No Title";

        var newPost = _contentService.Create(name: cleanTitle ?? "No Title", parentId: _blogRoot, documentTypeAlias: "blogArticle");
        newPost.SetCultureName(cleanTitle, "en-US");
        newPost.SetValue("postTitle", cleanTitle, "en-US");
        newPost.SetValue("shortDescription", simpleExcerpt, "en-US");
        newPost.SetValue("blogCategories", JsonConvert.SerializeObject(new[] { post.Category }));
        newPost.SetValue("bodyText", post.Content, "en-US");
        if (postFeaturedImage != null)
        {
          newPost.SetValue("imageForListing", postFeaturedImage.ToString(), "en-US");
        }
        newPost.SetValue("publishedDate", post.PublishedDate);
        _contentService.Save(newPost);
        Task.Delay(500).Wait();
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while importing blog posts.");
    }
  }
  #endregion
}