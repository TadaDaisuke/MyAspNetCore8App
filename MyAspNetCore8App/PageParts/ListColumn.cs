namespace MyAspNetCore8App.PageParts;

/// <summary>
/// 一覧表の列情報
/// </summary>
/// <param name="name">列名</param>
/// <param name="sortItem">並び替え項目名</param>
public class ListColumn(string name, string? sortItem = null)
{
    /// <summary>
    /// 列名
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 並び替え項目名
    /// </summary>
    public string? SortItem { get; } = sortItem;
}
