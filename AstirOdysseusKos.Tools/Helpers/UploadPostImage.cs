
#region Using Directives
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;
#endregion

namespace Eyewide.Tools.Helpers;
public class UploadPostImage : IUploadPostImage
{
  #region Fields
  private readonly IMediaService _mediaService;
  private readonly MediaFileManager _mediaFileManager;
  private readonly IShortStringHelper _shortStringHelper;
  private readonly MediaUrlGeneratorCollection _mediaUrlGeneratorCollection;
  private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
  private readonly ILogger<UploadPostImage> _logger;
  // private readonly UmbracoHelper _helper;
  #endregion
  #region Constructor
  public UploadPostImage(IMediaService mediaService, MediaFileManager mediaFileManager, ILogger<UploadPostImage> logger, MediaUrlGeneratorCollection mediaUrlGeneratorCollection, IShortStringHelper shortStringHelper, IContentTypeBaseServiceProvider contentTypeBaseServiceProvider)
  {
    _mediaService = mediaService;
    _mediaFileManager = mediaFileManager;
    _logger = logger;
    _mediaUrlGeneratorCollection = mediaUrlGeneratorCollection;
    _shortStringHelper = shortStringHelper;
    _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;    
  }
  #endregion

  #region Methods
  public Udi UploadMediaItem(string imageUrl, int parentId)
  {
    int guid = parentId;
    #region Generate File Name
    var fileName = Guid.NewGuid().ToString("N") + ".img";
    if (!string.IsNullOrWhiteSpace(imageUrl) && Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
    {
      var path = uri.LocalPath.TrimEnd('/');
      fileName = Path.GetFileName(path);
      fileName = string.IsNullOrEmpty(fileName)
          ? (Path.GetExtension(uri.AbsolutePath) is var ext && !string.IsNullOrEmpty(ext)
              ? Guid.NewGuid().ToString("N") + ext
              : Guid.NewGuid().ToString("N") + ".img")
          : WebUtility.UrlDecode(fileName);
    }
    #endregion
    // Check for an existing media item with same file name under the parent
    var existing = FindExistingMediaByFileName(parentId, fileName);
    if (existing != null)
    {
      _logger.LogDebug("Media item with file name '{FileName}' already exists (Key: {Key}). Returning existing UDI.", fileName, existing.Key);
      return Udi.Create(Constants.UdiEntityType.Media, existing.Key);
    }

    #region Download Image and Create Media Item
    var request = WebRequest.Create(imageUrl);
    using (var response = request.GetResponse())
    using (var stream = response.GetResponseStream())
    {
      IMedia mediaItem = _mediaService.CreateMediaWithIdentity(fileName, parentId: parentId, Constants.Conventions.MediaTypes.Image);
      mediaItem.SetValue(_mediaFileManager,_mediaUrlGeneratorCollection, _shortStringHelper, _contentTypeBaseServiceProvider, Constants.Conventions.Media.File, fileName, stream);
      _mediaService.Save(mediaItem);
      var mediaUdi = Udi.Create(Constants.UdiEntityType.Media, mediaItem.Key);
      return mediaUdi;
    }
    #endregion
  }

  private IMedia? FindExistingMediaByFileName(int parentId, string fileName)
  {
    try
    {
      // Get children of the parent; IMediaService exposes GetChildren
      var children = _mediaService.GetPagedChildren(parentId, 0, int.MaxValue, out long total).ToList();
      foreach (var child in children)
      {
        // Compare by media item Name first (some imports set the media name == file name)
        if (!string.IsNullOrWhiteSpace(child.Name) && string.Equals(child.Name, fileName, StringComparison.OrdinalIgnoreCase))
          return child;

        // Try to get the stored umbracoFile value
        var val = child.GetValue<string>(Constants.Conventions.Media.File);
        if (string.IsNullOrWhiteSpace(val))
          continue;

        // The value may be JSON ({"src":"/media/.../file.jpg", ...}) or a plain path like "/media/{id}/file.jpg"
        string? src = null;
        var trimmed = val.Trim();
        if (trimmed.StartsWith("{"))
        {
          try
          {
            using var doc = JsonDocument.Parse(trimmed);
            if (doc.RootElement.TryGetProperty("src", out var srcElem) && srcElem.ValueKind == JsonValueKind.String)
              src = srcElem.GetString();
          }
          catch
          {
            // ignore JSON parse errors and fall back to raw value
            src = null;
          }
        }

        if (src == null)
          src = trimmed.Trim('"');

        if (string.IsNullOrWhiteSpace(src))
          continue;

        var storedFileName = Path.GetFileName(src);
        if (!string.IsNullOrWhiteSpace(storedFileName) && string.Equals(storedFileName, fileName, StringComparison.OrdinalIgnoreCase))
          return child;
      }
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Failed while searching for existing media under parent {ParentId}", parentId);
    }

    return null;
  }
  #endregion
}
