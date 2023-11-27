namespace MyAspNetCore8App.Domain;

/// <summary>
/// 部署リポジトリーインターフェース
/// </summary>
public interface IDepartmentRepository
{
    /// <summary>
    /// 検索条件に合致する部署（一覧の行のコレクション）を返す。
    /// </summary>
    public IEnumerable<DepartmentListRow> SearchDepartments(DepartmentSearchCondition searchCondition);

    /// <summary>
    /// 部署全件を返す。
    /// </summary>
    public IEnumerable<Department> GetAllDepartments();

    /// <summary>
    /// 該当する部署コードの部署を返す。
    /// </summary>
    public Department? GetDepartment(string departmentCode);

    /// <summary>
    /// 部署情報を登録/更新する。
    /// </summary>
    public void SaveDepartment(Department department);

    /// <summary>
    /// 検索条件に合致する部署一覧のExcelファイルを生成し、そのバイト配列を返す。
    /// </summary>
    public byte[] CreateExcelBytes(DepartmentSearchCondition searchCondition, string sheetName);
}
