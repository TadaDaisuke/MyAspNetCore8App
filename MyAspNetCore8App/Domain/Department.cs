using System.ComponentModel.DataAnnotations;

namespace MyAspNetCore8App.Domain;

/// <summary>
/// 部署
/// </summary>
public class Department
{
    /// <summary>
    /// 部署コード
    /// </summary>
    [Display(Name = "部署コード")]
    [Required(AllowEmptyStrings = false)]
    public string? DepartmentCode { get; set; }

    /// <summary>
    /// 部署名
    /// </summary>
    [Display(Name = "部署名")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "部署名は入力必須です")]
    [RegularExpression(@"^\S+$", ErrorMessage = "空白文字は使用できません")]
    public string? DepartmentName { get; set; }
}
