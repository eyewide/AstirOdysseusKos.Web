namespace AstirOdysseusKos.Web.Models;

public class FeaturedImage
{
  public string? SourceUrl { get; set; } = string.Empty;
  public string? AltText { get; set; } = string.Empty;
  public string? Slug { get; set; } = string.Empty;
  public Dictionary<string, string>? MediaSizes { get; set; } = new();
}
