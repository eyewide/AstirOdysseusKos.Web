using Umbraco.Cms.Core;

namespace Eyewide.Tools.Helpers;

/// <summary>
/// Provides functionality to upload or register post images as Umbraco media items.
/// </summary>
public interface IUploadPostImage
{
  /// <summary>
  /// Uploads an image located at the specified <paramref name="imageUrl"/> into Umbraco's media library
  /// under the media node with the given <paramref name="parentId"/>, and returns the media item's UDI.
  /// </summary>
  /// <param name="imageUrl">
  /// The URL of the image to upload. This can be an absolute or relative URL that the implementation
  /// will retrieve and store as a media item.
  /// </param>
  /// <param name="parentId">
  /// The numeric identifier of the parent media node under which the new media item should be created.
  /// Implementations should use this to place the media in the correct folder or container.
  /// </param>
  /// <returns>
  /// A <see cref="Udi"/> that identifies the created (or existing) Umbraco media item corresponding to the uploaded image.
  /// </returns>
  /// <remarks>
  /// Implementations are responsible for validating the input, fetching the image data, creating or reusing
  /// an appropriate media item in Umbraco, and mapping the resulting media identifier to a UDI.
  /// </remarks>
  public Udi UploadMediaItem(string imageUrl, int parentId);
}
