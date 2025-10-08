using AstirOdysseusKos.Web.Models;

namespace AstirOdysseusKos.Web.Services;

public interface IBlogService
{
  Task<List<BlogPost>> GetAllBlogPostsAsync(int count, int skip = 0, int language = 1);
  Task<BlogPost> GetBlogPostByIdAsync(int id);
  Task<string> GetBlogFeaturedImage(int id);
}
