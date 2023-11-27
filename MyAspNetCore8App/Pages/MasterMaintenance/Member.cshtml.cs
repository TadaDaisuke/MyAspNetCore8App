using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyAspNetCore8App.Domain;
using MyAspNetCore8App.PageParts;

namespace MyAspNetCore8App.Pages.MasterMaintenance;

/// <summary>
/// Member�y�[�W���f��
/// </summary>
/// <param name="memberService">�����o�[�T�[�r�X</param>
/// <param name="departmentService">�����T�[�r�X</param>
public class MemberModel(IMemberService memberService, IDepartmentService departmentService) : PageModel
{
    /// <summary>
    /// �����h���b�v�_�E�����X�g�A�C�e���̃R���N�V����
    /// </summary>
    public IEnumerable<SelectListItem> DepartmentListItems { get; } = departmentService.GetAllDepartments()
        .Select(x => new SelectListItem { Value = x.DepartmentCode, Text = x.DepartmentName })
        .ToList();

    /// <summary>
    /// �d�q���[���h���C�����W�I�{�^���A�C�e���̃R���N�V����
    /// </summary>
    public IEnumerable<RadioItem> EmailDomains { get; } = new List<RadioItem>
        {
            new("�w��Ȃ�", "", true),
            new(".com", ".com"),
            new(".net", ".net"),
            new(".org", ".org"),
            new(".co.jp", ".co.jp"),
            new(".ne.jp", ".ne.jp"),
        };

    /// <summary>
    /// �����o�[��������
    /// </summary>
    [BindProperty]
    public MemberSearchCondition SearchCondition { get; set; } = new MemberSearchCondition();

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
        var members = memberService.SearchMembers(SearchCondition);
        Response.Headers.Append("X-total-records-count", (members.FirstOrDefault()?.TotalRecordsCount ?? 0).ToString());
        Response.Headers.Append("X-last-seq", members.Max(x => x?.Seq)?.ToString());
        return Partial("MemberList", members);
    }

    /// <summary>
    /// GetDetail�iPOST�j���N�G�X�g�n���h���[
    /// </summary>
    public PartialViewResult OnPostGetDetail([FromForm] string? detailKey)
    {
        ArgumentNullException.ThrowIfNull(detailKey);
        var member = memberService.GetMember(detailKey) ?? throw new InvalidOperationException(detailKey.ToString());
        member.DepartmentListItems = DepartmentListItems.ToList();
        return Partial("MemberDetail", member);
    }

    /// <summary>
    /// GetBlankDetail�iPOST�j���N�G�X�g�n���h���[
    /// </summary>
    public PartialViewResult OnPostGetBlankDetail()
    {
        var member = new Member
        {
            MemberCode = "�i�V�K�j",
            DepartmentListItems = DepartmentListItems.ToList()
        };
        return Partial("MemberDetail", member);
    }

    /// <summary>
    /// SaveDetail�iPOST�j���N�G�X�g�n���h���[
    /// </summary>
    public ContentResult OnPostSaveDetail([FromForm] Member? member)
    {
        ArgumentNullException.ThrowIfNull(member);
        if (!ModelState.IsValid)
        {
            throw new InvalidDataException(nameof(member));
        }
        memberService.SaveMember(member);
        return Content("�X�V���܂���");
    }

    /// <summary>
    /// DownloadExcel�iPOST�j���N�G�X�g�n���h���[
    /// </summary>
    public FileContentResult OnPostDownloadExcel()
    {
        var bytes = memberService.DownloadMembers(SearchCondition);
        Response.Headers.Append("X-download-file-name", $"Members_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        return File(bytes, CONTENT_TYPE_XLSX);
    }

    /// <summary>
    /// SuggestMemberCode�iPOST�j���N�G�X�g�n���h���[
    /// </summary>
    public JsonResult OnPostSuggestMemberCode(string? memberCodePart)
    {
        ArgumentNullException.ThrowIfNull(memberCodePart);
        var list = memberService.SuggestMemberCode(memberCodePart);
        return new JsonResult(list);
    }
}
