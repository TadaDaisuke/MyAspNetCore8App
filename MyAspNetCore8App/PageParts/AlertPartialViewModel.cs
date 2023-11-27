namespace MyAspNetCore8App.PageParts;

/// <summary>
/// _AlertPartialビューモデル
/// </summary>
/// <param name="alertType">アラート種別</param>
/// <param name="alertMessage">アラートメッセージ</param>
public class AlertPartialViewModel(AlertType alertType, string alertMessage)
{
    /// <summary>
    /// アラート種別
    /// </summary>
    public AlertType AlertType { get; } = alertType;

    /// <summary>
    /// アラートメッセージ
    /// </summary>
    public string AlertMessage { get; } = alertMessage;
}
