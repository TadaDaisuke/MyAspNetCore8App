namespace MyAspNetCore8App.PageParts;

/// <summary>
/// パンくずリストアイテム
/// </summary>
/// <param name="url">URL</param>
/// <param name="title">タイトル</param>
public class BreadCrumbItem(string url, string title)
{
    /// <summary>
    /// URL
    /// </summary>
    public string Url { get; } = url;

    /// <summary>
    /// タイトル
    /// </summary>
    public string Title { get; } = title;
}
