using System.Web;
using Umbraco.Cms.Core;

namespace AstirOdysseusKos.Tools.Model;

/// <summary>
/// Represents a blog post with metadata, content and related media information used by the application.
/// </summary>
/// <remarks>
/// Instances typically contain the post identifier, human-readable title and excerpt, the HTML-encoded
/// content and a decoded convenience property, routing slug, featured image metadata, an Umbraco UDI for
/// images pending upload, publication date, category and tags.
/// </remarks>
public class BlogPost
{
  /// <summary>
  /// Gets or sets the numeric identifier for the blog post.
  /// </summary>
  /// <value>
  /// An integer uniquely identifying the post within the application's data store.
  /// </value>
  public int Id { get; set; }

  /// <summary>
  /// Gets or sets the title of the blog post.
  /// </summary>
  /// <value>
  /// A short, human-readable headline for the post. Nullable to allow construction before a title is provided.
  /// </value>
  public string? Title { get; set; }

  /// <summary>
  /// Gets or sets the short excerpt or summary of the post.
  /// </summary>
  /// <value>
  /// A brief description or lead used in lists and previews. Nullable when an excerpt is not supplied.
  /// </value>
  public string? Excerpt { get; set; }

  /// <summary>
  /// Gets or sets the HTML-encoded content of the post.
  /// </summary>
  /// <value>
  /// The full post body stored as an HTML-encoded string. Use <see cref="Content"/> to obtain a decoded representation.
  /// Nullable when content has not been provided.
  /// </value>
  public string? EncodedContent { get; set; }

  /// <summary>
  /// Gets the HTML-decoded content of the post.
  /// </summary>
  /// <remarks>
  /// This convenience property returns the result of <see cref="HttpUtility.HtmlDecode(string?)"/>
  /// applied to <see cref="EncodedContent"/>. It may be <c>null</c> when <see cref="EncodedContent"/> is <c>null</c>.
  /// </remarks>
  public string? Content => HttpUtility.HtmlDecode(EncodedContent);

  /// <summary>
  /// Gets or sets the URL-friendly slug for the post.
  /// </summary>
  /// <value>
  /// A string suitable for use in routes or SEO-friendly URLs (for example, "my-post-title").
  /// Nullable to allow generation or assignment later.
  /// </value>
  public string? Slug { get; set; }

  /// <summary>
  /// Gets or sets the featured image metadata for the post.
  /// </summary>
  /// <value>
  /// A <see cref="FeaturedImage"/> instance containing the image source, alt text, slug and media size variants.
  /// Nullable when no featured image is specified.
  /// </value>
  public FeaturedImage? FeaturedImage { get; set; }

  /// <summary>
  /// Gets or sets an Umbraco UDI representing an image that should be uploaded or attached to the post.
  /// </summary>
  /// <value>
  /// A <see cref="Udi"/> instance used by the integration layer to identify or create media items in Umbraco.
  /// Nullable when there is no pending image upload.
  /// </value>
  public Udi? ImageToUpload { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the post was published.
  /// </summary>
  /// <value>
  /// A <see cref="DateTime"/> value representing the publication timestamp. Consumers may use UTC or local time
  /// according to application conventions.
  /// </value>
  public DateTime PublishedDate { get; set; }

  /// <summary>
  /// Gets or sets the category name for the post.
  /// </summary>
  /// <value>
  /// A single category string used to group or filter posts. Nullable when no category is assigned.
  /// </value>
  public string? Category { get; set; }

  /// <summary>
  /// Gets or sets the collection of tag names associated with the post.
  /// </summary>
  /// <value>
  /// An enumerable of tag strings (for example, "news", "tutorial"). Nullable when no tags are present.
  /// </value>
  public IEnumerable<string>? Tags { get; set; }
}
