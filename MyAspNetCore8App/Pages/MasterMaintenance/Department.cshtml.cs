using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAspNetCore8App.Domain;

namespace MyAspNetCore8App.Pages.MasterMaintenance;

/// <summary>
/// Departmentページモデル
/// </summary>
/// <param name="departmentService">部署サービス</param>
public class DepartmentModel(IDepartmentService departmentService) : PageModel
{
    /// <summary>
    /// 部署サービス
    /// </summary>
    private readonly IDepartmentService _departmentService = departmentService;

    /// <summary>
    /// 部署検索条件
    /// </summary>
    [BindProperty]
    public DepartmentSearchCondition SearchCondition { get; set; } = new DepartmentSearchCondition();

    /// <summary>
    /// GETリクエストハンドラー
    /// </summary>
    public void OnGet()
    {
    }

    /// <summary>
    /// Search（POST）リクエストハンドラー
    /// </summary>
    public PartialViewResult OnPostSearch()
    {
        var departments = _departmentService.SearchDepartments(SearchCondition);
        Response.Headers.Append("X-total-records-count", (departments.FirstOrDefault()?.TotalRecordsCount ?? 0).ToString());
        Response.Headers.Append("X-last-seq", departments.Max(x => x?.Seq)?.ToString());
        return Partial("DepartmentList", departments);
    }

    /// <summary>
    /// GetDetail（POST）リクエストハンドラー
    /// </summary>
    public PartialViewResult OnPostGetDetail([FromForm] string? detailKey)
    {
        ArgumentNullException.ThrowIfNull(detailKey);
        return Partial("DepartmentDetail", _departmentService.GetDepartment(detailKey));
    }

    /// <summary>
    /// GetBlankDetail（POST）リクエストハンドラー
    /// </summary>
    public PartialViewResult OnPostGetBlankDetail()
    {
        return Partial("DepartmentDetail", new Department { DepartmentCode = "（新規）" });
    }

    /// <summary>
    /// SaveDetail（POST）リクエストハンドラー
    /// </summary>
    public ContentResult OnPostSaveDetail([FromForm] Department? department)
    {
        ArgumentNullException.ThrowIfNull(department);
        if (!ModelState.IsValid)
        {
            throw new InvalidDataException(nameof(department));
        }
        _departmentService.SaveDepartment(department);
        return Content("更新しました");
    }

    /// <summary>
    /// DownloadExcel（POST）リクエストハンドラー
    /// </summary>
    public FileContentResult OnPostDownloadExcel()
    {
        var bytes = _departmentService.DownloadDepartments(SearchCondition);
        Response.Headers.Append("X-download-file-name", $"Departments_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        return File(bytes, CONTENT_TYPE_XLSX);
    }
}
