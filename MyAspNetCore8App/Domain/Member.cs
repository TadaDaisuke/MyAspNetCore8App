using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MyAspNetCore8App.Domain;

/// <summary>
/// メンバー
/// </summary>
public class Member
{
    /// <summary>
    /// メンバーコード
    /// </summary>
    [Display(Name = "メンバーコード")]
    [Required(AllowEmptyStrings = false)]
    public string? MemberCode { get; set; }

    /// <summary>
    /// 英字名
    /// </summary>
    [Display(Name = "英字名")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "英字名は入力必須です")]
    [RegularExpression("^[A-Za-z]+$", ErrorMessage = "半角英字で入力してください")]
    public string? GivenName { get; set; }

    /// <summary>
    /// 英字姓
    /// </summary>
    [Display(Name = "英字姓")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "英字姓は入力必須です")]
    [RegularExpression("^[A-Za-z]+$", ErrorMessage = "半角英字で入力してください")]
    public string? FamilyName { get; set; }

    /// <summary>
    /// 漢字名
    /// </summary>
    [Display(Name = "漢字名")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "漢字名は入力必須です")]
    [RegularExpression(@"^\S+$", ErrorMessage = "空白文字は使用できません")]
    public string? GivenNameKanji { get; set; }

    /// <summary>
    /// 漢字姓
    /// </summary>
    [Display(Name = "漢字姓")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "漢字姓は入力必須です")]
    [RegularExpression(@"^\S+$", ErrorMessage = "空白文字は使用できません")]
    public string? FamilyNameKanji { get; set; }

    /// <summary>
    /// カナ名
    /// </summary>
    [Display(Name = "カナ名")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "カナ名は入力必須です")]
    [RegularExpression("^[ァ-ヴー]+$", ErrorMessage = "全角カタカナで入力してください")]
    public string? GivenNameKana { get; set; }

    /// <summary>
    /// カナ姓
    /// </summary>
    [Display(Name = "カナ姓")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "カナ姓は入力必須です")]
    [RegularExpression("^[ァ-ヴー]+$", ErrorMessage = "全角カタカナで入力してください")]
    public string? FamilyNameKana { get; set; }

    /// <summary>
    /// メールアドレス
    /// </summary>
    [Display(Name = "メールアドレス")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "メールアドレスは入力必須です")]
    [EmailAddress(ErrorMessage = "正しいメールアドレスを入力してください")]
    public string? MailAddress { get; set; }

    /// <summary>
    /// 所属部署コード
    /// </summary>
    [Display(Name = "所属部署")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "所属部署は選択必須です")]
    public string? DepartmentCode { get; set; }

    /// <summary>
    /// 所属部署名
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// 所属部署ドロップダウンリストアイテムのコレクション
    /// </summary>
    public IEnumerable<SelectListItem>? DepartmentListItems { get; set; }

    /// <summary>
    /// 着任日
    /// </summary>
    [Display(Name = "着任日")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "着任日は入力必須です")]
    public DateOnly? JoinedDate { get; set; }

    /// <summary>
    /// 離任日
    /// </summary>
    [Display(Name = "離任日")]
    public DateOnly? TerminationDate { get; set; }

    /// <summary>
    /// 備考
    /// </summary>
    [Display(Name = "備考")]
    public string? Note { get; set; }

    /// <summary>
    /// 英字フルネーム
    /// </summary>
    public string FullName => $"{GivenName} {FamilyName}";

    /// <summary>
    /// カナフルネーム
    /// </summary>
    public string FullNameKana => $"{FamilyNameKana} {GivenNameKana}";

    /// <summary>
    /// 漢字フルネーム
    /// </summary>
    public string FullNameKanji => $"{FamilyNameKanji} {GivenNameKanji}";
}
