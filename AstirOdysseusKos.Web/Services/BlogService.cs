using System.Text.Json;
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
      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = new Uri($"{baseUrl}/posts?_fields=id,slug,title,excerpt,featured_media,date"),
        Headers =
        {
          { "Authorization", $"Basic {apiKey}" }
        },
      };

      var response = await _httpClient.SendAsync(request);
      response.EnsureSuccessStatusCode();

      var responseBody = await response.Content.ReadAsStringAsync();
      var options = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true,
      };
      var blogPosts = new List<BlogPost>();
      using JsonDocument doc = await JsonDocument.ParseAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseBody)));
      foreach (var post in doc.RootElement.EnumerateArray())
      {
        if (post.ValueKind == JsonValueKind.Object)
        {
          var blogPost = new BlogPost
          {
            Id = post.GetProperty("id").GetInt32(),
            Slug = post.GetProperty("slug").GetString(),
            Title = post.GetProperty("title").GetProperty("rendered").GetString(),
            Excerpt = post.GetProperty("excerpt").GetProperty("rendered").GetString(),
            PublishedDate = post.GetProperty("date").GetDateTime()
          };
          blogPost.FeaturedImage = await GetBlogFeaturedImage(post.GetProperty("featured_media").GetInt32());
          blogPosts.Add(blogPost);
        }
      }
      return blogPosts.OrderByDescending(x => x.PublishedDate).Take(count).ToList<BlogPost>();
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
    FeaturedImage featuredImage = new FeaturedImage();
    var request = new HttpRequestMessage
    {
      Method = HttpMethod.Get,
      RequestUri = new Uri($"{baseUrl}/media/{id}?_fields=source_url,alt_text,slug,media_details"),
      Headers =
      {
        { "Authorization", $"Basic {apiKey}" }
      }
    };

    var response = await _httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();
    var responseBody = await response.Content.ReadAsStringAsync();
    var options = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
    };

    using JsonDocument doc = await JsonDocument.ParseAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseBody)));
    if (doc.RootElement.TryGetProperty("source_url", out JsonElement featuredMediaElement))
    {
      featuredImage.SourceUrl = featuredMediaElement.GetString();
    }

    if (doc.RootElement.TryGetProperty("alt_text", out JsonElement featuredImageAlt))
    {
      featuredImage.AltText = featuredImageAlt.GetString();
    }

    if (doc.RootElement.TryGetProperty("slug", out JsonElement featuredImageSlug))
    {
      featuredImage.Slug = featuredImageSlug.GetString();
    }

    if (doc.RootElement.TryGetProperty("media_details", out JsonElement mediaDetailsElement))
    {
      if (mediaDetailsElement.TryGetProperty("sizes", out JsonElement sizesElement))
      {
        foreach (var size in sizesElement.EnumerateObject())
        {
          if (size.NameEquals("medium") || size.NameEquals("medium_large") || size.NameEquals("large"))
          {
            // size.Value is an object like { "source_url": "...", "width": ..., ... }
            if (size.Value.TryGetProperty("source_url", out JsonElement sourceUrlElement))
            {
              var sourceUrl = sourceUrlElement.GetString();

              if (!string.IsNullOrEmpty(sourceUrl))
              {
                featuredImage.MediaSizes[size.Name] = sourceUrl;
                
                var webpSourceUrl = System.Text.RegularExpressions.Regex.Replace(
                    sourceUrl,
                    @"\.(jpg|png)$",
                    ".$1.webp",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                featuredImage.MediaSizes[$"{size.Name}_webp"] = webpSourceUrl;
              }
            }
          }
        }
      }
    }

    return featuredImage;
  }

  public Task<BlogPost> GetBlogPostByIdAsync(int id) => throw new NotImplementedException();

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