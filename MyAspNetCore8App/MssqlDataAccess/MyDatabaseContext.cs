using Microsoft.Data.SqlClient;
using Serilog;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace MyAspNetCore8App.MssqlDataAccess;

/// <summary>
/// MyDatabaseアクセス関連のコンテキスト
/// </summary>
/// <param name="connectionString">DB接続文字列</param>
public class MyDatabaseContext(string connectionString)
{
    /// <summary>
    /// 一覧検索の1回あたり読込み行数
    /// </summary>
    public readonly int FETCH_ROW_SIZE = 50;

    /// <summary>
    /// SELECT文（SqlCommand）の実行結果をDictionaryのList形式で返すとともに、カラム情報を出力する。
    /// </summary>
    /// <param name="cmd">SqlCommandオブジェクト</param>
    /// <param name="schemaTable">カラム情報出力用DataTable</param>
    public List<Dictionary<string, string?>> GetRowList(SqlCommand cmd, out DataTable schemaTable)
    {
        var list = new List<Dictionary<string, string?>>();
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            cmd.Connection = conn;
            using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.CloseConnection))
            {
                schemaTable = reader.GetSchemaTable();
                var uniqueColNames = new List<string>();
                foreach (var colName in schemaTable.AsEnumerable().Select(row => row.Field<string>("ColumnName")))
                {
                    uniqueColNames.Add((string.IsNullOrWhiteSpace(colName) || uniqueColNames.Contains(colName)) ? Guid.NewGuid().ToString() : colName);
                }
                while (reader.Read())
                {
                    var dict = new Dictionary<string, string?>(reader.FieldCount);
                    for (var index = 0; index < reader.FieldCount; index++)
                    {
                        var colValue = reader.GetValue(index);
                        dict.Add
                        (
                            uniqueColNames[index],
                            (colValue == null || colValue == DBNull.Value)
                                ? null
                                : colValue is DateTime dateTimeValue
                                    ? dateTimeValue.ToString(reader.GetDataTypeName(index) == "date" ? DEFAULT_DATEONLY_FORMAT : DEFAULT_DATETIME_FORMAT)
                                    : colValue.ToString()
                        );
                    }
                    list.Add(dict);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// SELECT文（SqlCommand）の実行結果をDictionaryのList形式で返す。
    /// </summary>
    /// <param name="cmd">SqlCommandオブジェクト</param>
    public List<Dictionary<string, string?>> GetRowList(SqlCommand cmd) => GetRowList(cmd, out _);

    /// <summary>
    /// SQL（SqlCommand）を実行する。
    /// </summary>
    /// <param name="cmd">SqlCommandオブジェクト</param>
    /// <returns>SqlCommand自身への参照</returns>
    public SqlCommand ExecuteSql(SqlCommand cmd)
    {
        var callerMethod = new StackFrame(1)?.GetMethod();
        var logMessage = new StringBuilder()
            .AppendLine($"SQLを実行します。呼び出し元: {callerMethod?.ReflectedType?.FullName}.{callerMethod?.Name}")
            .Append(GetCommandLogString(cmd))
            .ToString();
        Log.ForContext(GetType()).Information(logMessage);
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
        }
        return cmd;
    }

    /// <summary>
    /// SqlCommandオブジェクトのログ出力用文字列（CommandText/CommandType/Parameters）を返す。
    /// </summary>
    /// <param name="cmd">SqlCommandオブジェクト</param>
    private static string GetCommandLogString(SqlCommand cmd)
    {
        var sb = new StringBuilder()
            .AppendLine("[CommandText]")
            .Append(cmd.CommandType == CommandType.Text ? "" : "    ")
            .AppendLine(cmd.CommandText)
            .AppendLine("[CommandType]")
            .Append($"    {cmd.CommandType}");
        if (cmd.Parameters.Count > 0)
        {
            static string? formatValue(object? o) =>
                (o as DateTime?)?.ToString(DEFAULT_DATETIME_FORMAT)
                    ?? (o as DateOnly?)?.ToString(DEFAULT_DATEONLY_FORMAT)
                        ?? o?.ToString();
            sb.AppendLine().AppendLine("[Parameters]").Append(string.Join(
                Environment.NewLine,
                cmd.Parameters
                    .OfType<SqlParameter>()
                    .Select(p => $"    {p.ParameterName}({p.SqlDbType}): {formatValue(p.Value)}")));
        }
        return sb.ToString();
    }
}
