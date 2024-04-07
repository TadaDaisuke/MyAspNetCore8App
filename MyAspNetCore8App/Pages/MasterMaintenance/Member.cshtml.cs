using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyAspNetCore8App.Domain;
using MyAspNetCore8App.PageParts;

namespace MyAspNetCore8App.Pages.MasterMaintenance;

/// <summary>
/// Memberページモデル
/// </summary>
/// <param name="memberService">メンバーサービス</param>
/// <param name="departmentService">部署サービス</param>
public class MemberModel(IMemberService memberService, IDepartmentService departmentService) : PageModel
{
    /// <summary>
    /// 部署ドロップダウンリストアイテムのコレクション
    /// </summary>
    public IEnumerable<SelectListItem> DepartmentListItems { get; } = departmentService.GetAllDepartments()
        .Select(x => new SelectListItem { Value = x.DepartmentCode, Text = x.DepartmentName });

    /// <summary>
    /// 電子メールドメインラジオボタンアイテムのコレクション
    /// </summary>
    public IEnumerable<RadioItem> EmailDomains { get; } =
        [
            new("指定なし", "", true),
            new(".com", ".com"),
            new(".net", ".net"),
            new(".org", ".org"),
            new(".co.jp", ".co.jp"),
            new(".ne.jp", ".ne.jp"),
        ];

    /// <summary>
    /// メンバー検索条件
    /// </summary>
    [BindProperty]
    public MemberSearchCondition SearchCondition { get; set; } = new MemberSearchCondition();

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
        var members = memberService.SearchMembers(SearchCondition);
        Response.Headers.Append("X-total-records-count", (members.FirstOrDefault()?.TotalRecordsCount ?? 0).ToString());
        Response.Headers.Append("X-last-seq", members.Max(x => x?.Seq)?.ToString());
        return Partial("MemberList", members);
    }

    /// <summary>
    /// GetDetail（POST）リクエストハンドラー
    /// </summary>
    public PartialViewResult OnPostGetDetail([FromForm] string? detailKey)
    {
        ArgumentNullException.ThrowIfNull(detailKey);
        var member = memberService.GetMember(detailKey) ?? throw new InvalidOperationException(detailKey.ToString());
        member.DepartmentListItems = DepartmentListItems.ToList();
        return Partial("MemberDetail", member);
    }

    /// <summary>
    /// GetBlankDetail（POST）リクエストハンドラー
    /// </summary>
    public PartialViewResult OnPostGetBlankDetail()
    {
        var member = new Member
        {
            MemberCode = "（新規）",
            DepartmentListItems = DepartmentListItems.ToList()
        };
        return Partial("MemberDetail", member);
    }

    /// <summary>
    /// SaveDetail（POST）リクエストハンドラー
    /// </summary>
    public ContentResult OnPostSaveDetail([FromForm] Member? member)
    {
        ArgumentNullException.ThrowIfNull(member);
        if (!ModelState.IsValid)
        {
            throw new InvalidDataException(nameof(member));
        }
        memberService.SaveMember(member);
        return Content("更新しました");
    }

    /// <summary>
    /// DownloadExcel（POST）リクエストハンドラー
    /// </summary>
    public FileContentResult OnPostDownloadExcel()
    {
        var bytes = memberService.DownloadMembers(SearchCondition);
        Response.Headers.Append("X-download-file-name", $"Members_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        return File(bytes, CONTENT_TYPE_XLSX);
    }

    /// <summary>
    /// SuggestMemberCode（POST）リクエストハンドラー
    /// </summary>
    public JsonResult OnPostSuggestMemberCode(string? memberCodePart)
    {
        ArgumentNullException.ThrowIfNull(memberCodePart);
        var list = memberService.SuggestMemberCode(memberCodePart);
        return new JsonResult(list);
    }
}
