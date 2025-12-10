using AstirOdysseusKos.Tools.Model;

namespace AstirOdysseusKos.Tools.Client;
public interface IBlogService
{
  Task<List<BlogPost>> GetAllBlogPostsAsync(int count, int skip, int language = 1);
  Task<BlogPost> GetBlogPostByIdAsync(int id);
  Task<FeaturedImage> GetBlogFeaturedImage(int id);
}
