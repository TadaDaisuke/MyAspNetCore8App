using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyAspNetCore8App.Pages;

/// <summary>
/// Errorページモデル
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel() : PageModel
{
    /// <summary>
    /// GETリクエストハンドラー
    /// </summary>
    public void OnGet()
    {
    }
}
