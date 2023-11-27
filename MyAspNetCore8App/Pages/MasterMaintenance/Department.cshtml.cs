using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyAspNetCore8App.Domain;

namespace MyAspNetCore8App.Pages.MasterMaintenance;

/// <summary>
/// Department�y�[�W���f��
/// </summary>
/// <param name="departmentService">�����T�[�r�X</param>
public class DepartmentModel(IDepartmentService departmentService) : PageModel
{
    /// <summary>
    /// �����T�[�r�X
    /// </summary>
    private readonly IDepartmentService _departmentService = departmentService;

    /// <summary>
    /// ������������
    /// </summary>
    [BindProperty]
    public DepartmentSearchCondition SearchCondition { get; set; } = new DepartmentSearchCondition();

    /// <summary>
    /// GET���N�G�X�g�n���h���[
    /// </summary>
    public void OnGet()
    {
    }

    /// <summary>
    /// Search�iPOST�j���N�G�X�g�n���h���[
    /// </summary>
    public PartialViewResult OnPostSearch()
    {
        var departments = _departmentService.SearchDepartments(SearchCondition);
        Response.Headers.Append("X-total-records-count", (departments.FirstOrDefault()?.TotalRecordsCount ?? 0).ToString());
        Response.Headers.Append("X-last-seq", departments.Max(x => x?.Seq)?.ToString());
        return Partial("DepartmentList", departments);
    }

    /// <summary>
    /// GetDetail�iPOST�j���N�G�X�g�n���h���[
    /// </summary>
    public PartialViewResult OnPostGetDetail([FromForm] string? detailKey)
    {
        ArgumentNullException.ThrowIfNull(detailKey);
        return Partial("DepartmentDetail", _departmentService.GetDepartment(detailKey));
    }

    /// <summary>
    /// GetBlankDetail�iPOST�j���N�G�X�g�n���h���[
    /// </summary>
    public PartialViewResult OnPostGetBlankDetail()
    {
        return Partial("DepartmentDetail", new Department { DepartmentCode = "�i�V�K�j" });
    }

    /// <summary>
    /// SaveDetail�iPOST�j���N�G�X�g�n���h���[
    /// </summary>
    public ContentResult OnPostSaveDetail([FromForm] Department? department)
    {
        ArgumentNullException.ThrowIfNull(department);
        if (!ModelState.IsValid)
        {
            throw new InvalidDataException(nameof(department));
        }
        _departmentService.SaveDepartment(department);
        return Content("�X�V���܂���");
    }

    /// <summary>
    /// DownloadExcel�iPOST�j���N�G�X�g�n���h���[
    /// </summary>
    public FileContentResult OnPostDownloadExcel()
    {
        var bytes = _departmentService.DownloadDepartments(SearchCondition);
        Response.Headers.Append("X-download-file-name", $"Departments_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        return File(bytes, CONTENT_TYPE_XLSX);
    }
}
