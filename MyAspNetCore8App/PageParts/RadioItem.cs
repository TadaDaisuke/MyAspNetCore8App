namespace MyAspNetCore8App.PageParts;

/// <summary>
/// ラジオボタンアイテム
/// </summary>
/// <param name="label">ラベル</param>
/// <param name="value">値</param>
/// <param name="isChecked">[オプション] true: チェックあり, false: チェックなし（既定値 = false）。</param>
public class RadioItem(string label, string value, bool isChecked = false)
{
    /// <summary>
    /// ラベル
    /// </summary>
    public string Label { get; } = label;

    /// <summary>
    /// 値
    /// </summary>
    public string Value { get; } = value;

    /// <summary>
    /// チェック有無
    /// </summary>
    public bool IsChecked { get; } = isChecked;
}
