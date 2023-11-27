namespace MyAspNetCore8App.Utilities;

/// <summary>
/// Excel出力用設定
/// </summary>
public class ExcelSettings
{
    /// <summary>
    /// フォント名
    /// </summary>
    public string FontName { get; set; } = string.Empty;

    /// <summary>
    /// フォントサイズ
    /// </summary>
    public string? FontSize { get; set; }
}
