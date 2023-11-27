using System.ComponentModel.DataAnnotations;

namespace MyAspNetCore8App.Domain;

/// <summary>
/// メンバー検索条件
/// </summary>
public class MemberSearchCondition
{
    /// <summary>
    /// 読込み済み行数
    /// </summary>
    public int OffsetRows { get; set; } = 0;

    /// <summary>
    /// 並替え項目
    /// </summary>
    public string? SortItem { get; set; }

    /// <summary>
    /// 昇順/降順区分
    /// </summary>
    public string? SortType { get; set; }

    /// <summary>
    /// 氏名の一部
    /// </summary>
    [Display(Name = "氏名の一部")]
    public string? MemberNamePart { get; set; }

    /// <summary>
    /// メンバーコード
    /// </summary>
    [Display(Name = "メンバーコード")]
    [StringLength(8)]
    public string? MemberCode { get; set; }

    /// <summary>
    /// 着任日 From
    /// </summary>
    [Display(Name = "着任日 From")]
    public DateOnly? JoinedDateFrom { get; set; }

    /// <summary>
    /// 着任日 To
    /// </summary>
    [Display(Name = "着任日 To")]
    public DateOnly? JoinedDateTo { get; set; }

    /// <summary>
    /// 所属部署
    /// </summary>
    [Display(Name = "所属部署")]
    public string? DepartmentCode { get; set; }

    /// <summary>
    /// 離任者を含む
    /// </summary>
    [Display(Name = "離任者を含む")]
    public bool HasTerminatedMembers { get; set; } = true;

    /// <summary>
    /// メールアドレスのドメイン
    /// </summary>
    [Display(Name = "メールアドレスのドメイン")]
    public string? EmailDomain { get; set; }
}
