namespace AstirOdysseusKos.Tools.Helpers;

/// <summary>
/// Provides a read-only mapping of blog category identifiers to their display names.
/// </summary>
/// <remarks>
/// The <see cref="BlogCategories"/> record exposes the <see cref="Categories"/> property,
/// which is initialized with a set of commonly used blog categories. The property is typed
/// as <see cref="IReadOnlyDictionary{TKey, TValue}"/> to indicate callers should treat the
/// mapping as read-only. The underlying instance is a mutable <see cref="Dictionary{TKey, TValue}"/>,
/// so if true runtime immutability is required consider replacing the initializer with an
/// immutable collection or a <see cref="System.Collections.ObjectModel.ReadOnlyDictionary{TKey, TValue}"/>.
/// </remarks>
record BlogCategories
{
  /// <summary>
  /// Gets the mapping of category identifier to category name.
  /// </summary>
  /// <value>
  /// A read-only dictionary where the key is the numeric category identifier (for example, an external CMS id)
  /// and the value is the display name for that category.
  /// </value>
  /// <example>
  /// <code>
  /// // Retrieve the display name for category id 23:
  /// var name = new BlogCategories().Categories[23]; // "Destination"
  /// </code>
  /// </example>
  public IReadOnlyDictionary<int, string> Categories { get; } = new Dictionary<int, string>
  {
    { 23, "Destination" },
    { 24, "The Hotel" },
    { 32, "Gastronomy" },
    { 42, "Events & Activities" }
  };
}
