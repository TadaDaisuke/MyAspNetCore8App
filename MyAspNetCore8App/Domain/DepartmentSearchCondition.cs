using System.ComponentModel.DataAnnotations;

namespace MyAspNetCore8App.Domain;

/// <summary>
/// 部署検索条件
/// </summary>
public class DepartmentSearchCondition
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
    /// 部署名の一部
    /// </summary>
    [Display(Name = "部署名の一部")]
    public string? DepartmentNamePart { get; set; }
}
