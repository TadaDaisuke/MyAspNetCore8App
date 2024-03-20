using Microsoft.Data.SqlClient;
using MyAspNetCore8App.Domain;
using MyAspNetCore8App.MssqlDataAccess.Utilities;
using System.Data;

namespace MyAspNetCore8App.MssqlDataAccess;

/// <summary>
/// 部署リポジトリー実装クラス
/// </summary>
/// <param name="context">SQL Server データベースアクセス用のコンテキスト</param>
/// <param name="excelCreator">Excel生成ユーティリティー</param>
public class MssqlDepartmentRepository(MssqlContext context, IExcelCreator excelCreator) : IDepartmentRepository
{
    /// <inheritdoc/>
    public IEnumerable<DepartmentListRow> SearchDepartments(DepartmentSearchCondition searchCondition)
    {
        var cmd = new SqlCommand("sp_search_departments") { CommandType = CommandType.StoredProcedure }
            .AddParameter("@department_name_part", SqlDbType.NVarChar, searchCondition.DepartmentNamePart)
            .AddParameter("@sort_item", SqlDbType.NVarChar, searchCondition.SortItem)
            .AddParameter("@sort_type", SqlDbType.NVarChar, searchCondition.SortType)
            .AddParameter("@offset_rows", SqlDbType.Int, searchCondition.OffsetRows)
            .AddParameter("@fetch_rows", SqlDbType.Int, context.FETCH_ROW_SIZE);
        return context.GetRowList(cmd)
            .Select(row =>
                new DepartmentListRow(
                    department: new Department { DepartmentCode = row["department_code"], DepartmentName = row["department_name"] },
                    seq: row["seq"].ToInt(),
                    totalRecordsCount: row["total_records_count"].ToInt()))
            .ToList();
    }

    /// <inheritdoc/>
    public IEnumerable<Department> GetAllDepartments()
    {
        var cmd = new SqlCommand("sp_search_departments") { CommandType = CommandType.StoredProcedure }
            .AddParameter("@department_name_part", SqlDbType.NVarChar, null)
            .AddParameter("@sort_item", SqlDbType.NVarChar, null)
            .AddParameter("@sort_type", SqlDbType.NVarChar, null)
            .AddParameter("@offset_rows", SqlDbType.Int, 0)
            .AddParameter("@fetch_rows", SqlDbType.Int, int.MaxValue);
        return context.GetRowList(cmd)
            .Select(row => new Department { DepartmentCode = row["department_code"], DepartmentName = row["department_name"] })
            .ToList();
    }

    /// <inheritdoc/>
    public Department? GetDepartment(string departmentCode)
    {
        var cmd = new SqlCommand("sp_get_department") { CommandType = CommandType.StoredProcedure }
            .AddParameter("@department_code", SqlDbType.NVarChar, departmentCode);
        return context.GetRowList(cmd)
            .Select(row => new Department { DepartmentCode = row["department_code"], DepartmentName = row["department_name"] })
            .FirstOrDefault();
    }

    /// <inheritdoc/>
    public void SaveDepartment(Department department)
    {
        var cmd = new SqlCommand("sp_save_department") { CommandType = CommandType.StoredProcedure }
            .AddParameter("@department_code", SqlDbType.NVarChar, department.DepartmentCode)
            .AddParameter("@department_name", SqlDbType.NVarChar, department.DepartmentName);
        context.ExecuteSqlCommand(cmd);
    }

    /// <inheritdoc/>
    public byte[] CreateExcelBytes(DepartmentSearchCondition searchCondition, string sheetName)
    {
        var cmd = new SqlCommand("sp_download_departments") { CommandType = CommandType.StoredProcedure }
            .AddParameter("@department_name_part", SqlDbType.NVarChar, searchCondition.DepartmentNamePart)
            .AddParameter("@sort_item", SqlDbType.NVarChar, searchCondition.SortItem)
            .AddParameter("@sort_type", SqlDbType.NVarChar, searchCondition.SortType);
        return excelCreator.CreateFileBytes(cmd, sheetName);
    }
}
