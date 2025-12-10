namespace AstirOdysseusKos.Tools.Model;

/// <summary>
/// Represents a featured image with metadata used by the application.
/// </summary>
/// <remarks>
/// Instances typically contain the image source URL, alternative text for accessibility,
/// an optional slug used as an identifier or for SEO, and a dictionary of media size
/// variants (for example "thumbnail" or "medium" mapped to their respective URLs).
/// Properties are nullable to accommodate deserialization scenarios, but defaults are
/// provided to avoid null-reference usage in common cases.
/// </remarks>
public class FeaturedImage
{
  /// <summary>
  /// Gets or sets the image source URL.
  /// </summary>
  /// <value>
  /// An absolute or relative URL pointing to the image resource. Defaults to an empty string.
  /// </value>
  public string? SourceUrl { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the alternative text for the image.
  /// </summary>
  /// <value>
  /// A short textual description used for accessibility (screen readers) and when the image cannot be displayed.
  /// Defaults to an empty string.
  /// </value>
  public string? AltText { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the slug or short identifier for the image.
  /// </summary>
  /// <value>
  /// A URL-friendly identifier or name used for routing, SEO, or internal reference. Defaults to an empty string.
  /// </value>
  public string? Slug { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets a mapping of media size keys to image URLs.
  /// </summary>
  /// <value>
  /// A dictionary where keys represent named sizes (for example, "thumbnail", "small", "large")
  /// and values are the corresponding image URLs. Defaults to an empty dictionary.
  /// </value>
  public Dictionary<string, string>? MediaSizes { get; set; } = new();
}
