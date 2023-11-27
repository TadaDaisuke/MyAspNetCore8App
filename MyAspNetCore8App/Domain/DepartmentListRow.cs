namespace MyAspNetCore8App.Domain;

/// <summary>
/// 部署一覧の行
/// </summary>
/// <param name="department">部署</param>
/// <param name="seq">行番号</param>
/// <param name="totalRecordsCount">総行数</param>
public class DepartmentListRow(Department department, int seq, int totalRecordsCount)
{
    /// <summary>
    /// 部署
    /// </summary>
    public Department Department { get; } = department;

    /// <summary>
    /// 行番号
    /// </summary>
    public int Seq { get; } = seq;

    /// <summary>
    /// 総行数
    /// </summary>
    public int TotalRecordsCount { get; } = totalRecordsCount;
}
