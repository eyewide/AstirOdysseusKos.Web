using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using AstirOdysseusKos.Web.Models;
namespace AstirOdysseusKos.Web.Services;
public class BlogService : IBlogService
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<BlogService> _logger;
  private readonly IConfiguration _configuration;
  public BlogService(HttpClient httpClient, ILogger<BlogService> logger, IConfiguration configuration)
  {
    _httpClient = httpClient;
    _logger = logger;
    _configuration = configuration;
  }
  public async Task<List<BlogPost>> GetAllBlogPostsAsync(int count, int skip = 0, int language = 1)
  {
    var baseUrl = GetBaseUrl();
    var apiKey = GetApiKey();
    try
    {
      var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/posts?_fields=id,slug,title,excerpt,featured_media,date");
      request.Headers.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
      var response = await _httpClient.SendAsync(request);
      response.EnsureSuccessStatusCode();
      using var stream = await response.Content.ReadAsStreamAsync();
      using var doc = await JsonDocument.ParseAsync(stream);
      var blogPosts = new List<BlogPost>();
      var imageFetchTasks = new List<Task>();
      var semaphore = new SemaphoreSlim(5); // Limit concurrent image fetches
      foreach (var post in doc.RootElement.EnumerateArray())
      {
        if (post.ValueKind != JsonValueKind.Object)
          continue;
        var blogPost = new BlogPost
        {
          Id = post.TryGetProperty("id", out var idProp) && idProp.TryGetInt32(out int idVal) ? idVal : 0,
          Slug = post.TryGetProperty("slug", out var slugProp) ? slugProp.GetString() : string.Empty,
          Title = post.TryGetProperty("title", out var titleProp) && titleProp.TryGetProperty("rendered", out var titleRendered) ? titleRendered.GetString() : string.Empty,
          Excerpt = post.TryGetProperty("excerpt", out var excerptProp) && excerptProp.TryGetProperty("rendered", out var excerptRendered) ? excerptRendered.GetString() : string.Empty,
          PublishedDate = post.TryGetProperty("date", out var dateProp) && dateProp.ValueKind == JsonValueKind.String && DateTime.TryParse(dateProp.GetString(), out DateTime dateVal) ? dateVal : DateTime.MinValue
        };
        if (post.TryGetProperty("featured_media", out var fmProp) && fmProp.TryGetInt32(out int mediaId) && mediaId > 0)
        {
          var t = Task.Run(async () =>
          {
            await semaphore.WaitAsync();
            try
            {
              blogPost.FeaturedImage = await GetBlogFeaturedImage(mediaId);
            }
            finally
            {
              semaphore.Release();
            }
          });
          imageFetchTasks.Add(t);
        }
        blogPosts.Add(blogPost);
      }
      await Task.WhenAll(imageFetchTasks);
      return blogPosts
        .OrderByDescending(bp => bp.PublishedDate)
        .Skip(skip)
        .Take(count)
        .ToList();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching blog posts");
      return new List<BlogPost>();
    }
  }
  public async Task<FeaturedImage> GetBlogFeaturedImage(int id)
  {
    var baseUrl = GetBaseUrl();
    var apiKey = GetApiKey();
    var featuredImage = new FeaturedImage { MediaSizes = new Dictionary<string, string>() };
    var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/media/{id}?_fields=source_url,alt_text,slug,media_details");
    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
    using var response = await _httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();
    using var stream = await response.Content.ReadAsStreamAsync();    
    using JsonDocument doc = await JsonDocument.ParseAsync(stream);
    if (doc.RootElement.TryGetProperty("source_url", out var src)) featuredImage.SourceUrl = src.GetString() ?? string.Empty;
    if (doc.RootElement.TryGetProperty("alt_text", out var alt)) featuredImage.AltText = alt.GetString() ?? string.Empty;
    if (doc.RootElement.TryGetProperty("slug", out var slug)) featuredImage.Slug = slug.GetString() ?? string.Empty;
    if (doc.RootElement.TryGetProperty("media_details",  out var mediaDetails) && mediaDetails.TryGetProperty("sizes", out var sizes))
    {
      foreach (var size in sizes.EnumerateObject())
      {
        if (size.NameEquals("medium") || size.NameEquals("medium_large") || size.NameEquals("large") || size.NameEquals("full"))
        {
          if (size.Value.TryGetProperty("source_url", out var sizeUrl))
          {
            var sourceUrl = sizeUrl.GetString();
            if (!string.IsNullOrEmpty(sourceUrl))
            {
              featuredImage.MediaSizes ??= new Dictionary<string, string>();
              featuredImage.MediaSizes[size.Name] = sourceUrl;
              var webpSourceUrl = Regex.Replace(sourceUrl, @"\.(jpg|png)$", ".$1.webp", RegexOptions.IgnoreCase);
              featuredImage.MediaSizes[$"{size.Name}_webp"] = webpSourceUrl;
            }
          }
        }
      }
    }
    return featuredImage;
  }
  public async Task<BlogPost> GetBlogPostByIdAsync(int id)
  {
    var baseUrl = GetBaseUrl();
    var apiKey = GetApiKey();
    try
    {
      var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/posts/{id}?_fields=id,slug,title,excerpt,featured_media,date");
      request.Headers.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
      var response = await _httpClient.SendAsync(request);
      response.EnsureSuccessStatusCode();
      using var stream = await response.Content.ReadAsStreamAsync();
      using var doc = await JsonDocument.ParseAsync(stream);
      var root = doc.RootElement;
      if (root.ValueKind != JsonValueKind.Object)
        return new BlogPost();
      var blogPost = new BlogPost
      {
        Id = root.TryGetProperty("id", out var idProp) && idProp.TryGetInt32(out int idVal) ? idVal : 0,
        Slug = root.TryGetProperty("slug", out var slugProp) ? slugProp.GetString() ?? string.Empty : string.Empty,
        Title = root.TryGetProperty("title", out var titleProp) && titleProp.TryGetProperty("rendered", out var titleRendered) ? titleRendered.GetString() : string.Empty,
        Excerpt = root.TryGetProperty("excerpt", out var excerptProp) && excerptProp.TryGetProperty("rendered", out var excerptRendered) ? excerptRendered.GetString() : string.Empty,
        PublishedDate = root.TryGetProperty("date", out var dateProp) && dateProp.ValueKind == JsonValueKind.String && DateTime.TryParse(dateProp.GetString(), out DateTime dateVal) ? dateVal : DateTime.MinValue
      };
      if (root.TryGetProperty("featured_media", out var fmProp) && fmProp.TryGetInt32(out int mediaId) && mediaId > 0)
      {
        try
        {
          blogPost.FeaturedImage = await GetBlogFeaturedImage(mediaId);
        }
        catch (Exception ex)
        {
          // If featured image fetch fails, log but still return the post
          _logger.LogWarning(ex, "Failed to fetch featured image for post {PostId} (media id {MediaId})", id, mediaId);
        }
      }
      return blogPost;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching blog post with id {Id}", id);
      return new BlogPost();
    }
  }
  private string GetApiKey()
  {
    var apiUsername = _configuration["BlogSettings:Username"];
    var apiPassword = _configuration["BlogSettings:Password"];
    var credentials = $"{apiUsername}:{apiPassword}";
    return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));
  }
  private string GetBaseUrl()
  {
    return _configuration["BlogSettings:BaseUrl"];
  }
}