using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyAspNetCore8App.Pages;

/// <summary>
/// Error�y�[�W���f��
/// </summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel() : PageModel
{
    /// <summary>
    /// GET���N�G�X�g�n���h���[
    /// </summary>
    public void OnGet()
    {
    }
}
