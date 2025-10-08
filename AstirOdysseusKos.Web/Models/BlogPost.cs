namespace AstirOdysseusKos.Web.Models;

public class BlogPost
{
  public int Id { get; set; }
  public string? Title { get; set; }
  public string? Excerpt { get; set; }
  public string? Slug { get; set; }
  public string? FeaturedImage { get; set; }
  public DateTime PublishedDate { get; set; }
}
