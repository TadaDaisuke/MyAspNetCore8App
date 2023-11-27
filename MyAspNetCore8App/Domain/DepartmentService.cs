namespace MyAspNetCore8App.Domain;

/// <summary>
/// 部署サービス実装クラス
/// </summary>
/// <param name="departmentRepository">部署リポジトリー</param>
public class DepartmentService(IDepartmentRepository departmentRepository) : IDepartmentService
{
    /// <inheritdoc/>
    public IEnumerable<DepartmentListRow> SearchDepartments(DepartmentSearchCondition searchCondition)
        => departmentRepository.SearchDepartments(searchCondition);

    /// <inheritdoc/>
    public IEnumerable<Department> GetAllDepartments()
        => departmentRepository.GetAllDepartments();

    /// <inheritdoc/>
    public Department? GetDepartment(string departmentCode)
        => departmentRepository.GetDepartment(departmentCode);

    /// <inheritdoc/>
    public void SaveDepartment(Department department)
        => departmentRepository.SaveDepartment(department);

    /// <inheritdoc/>
    public byte[] DownloadDepartments(DepartmentSearchCondition searchCondition)
        => departmentRepository.CreateExcelBytes(searchCondition, "Departments");
}
