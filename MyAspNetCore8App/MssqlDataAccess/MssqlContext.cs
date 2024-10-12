using Microsoft.Data.SqlClient;
using Serilog;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace MyAspNetCore8App.MssqlDataAccess;

/// <summary>
/// SQL Server データベースアクセス用のコンテキスト
/// </summary>
/// <param name="connectionString">DB接続文字列</param>
public class MssqlContext(string connectionString)
{
    /// <summary>
    /// 一覧検索の1回あたり読込み行数
    /// </summary>
    public readonly int FETCH_ROW_SIZE = 50;

    /// <summary>
    /// SELECT文（SqlCommand）の実行結果をDictionaryのList形式で返すとともに、カラムのメタデータを出力する。
    /// </summary>
    /// <param name="cmd">SqlCommandオブジェクト</param>
    /// <param name="columnSchemas">カラムのメタデータのリスト（出力用）</param>
    public IList<IDictionary<string, string?>> GetRowList(SqlCommand cmd, out IList<ColumnSchema> columnSchemas)
    {
        var list = new List<IDictionary<string, string?>>();
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            cmd.Connection = conn;
            using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult | CommandBehavior.CloseConnection))
            {
                columnSchemas = reader.GetSchemaTable().AsEnumerable()
                    .Select(row =>
                        new ColumnSchema(
                            row.Field<string>("ColumnName") ?? string.Empty,
                            row.Field<string>("DataTypeName") ?? string.Empty,
                            row.Field<short>("NumericScale")))
                    .ToList();
                var uniqueColNames = new List<string>();
                foreach (var columnSchema in columnSchemas)
                {
                    uniqueColNames.Add(
                        string.IsNullOrWhiteSpace(columnSchema.ColumnName) || uniqueColNames.Contains(columnSchema.ColumnName)
                            ? Guid.NewGuid().ToString()
                            : columnSchema.ColumnName);
                }
                while (reader.Read())
                {
                    var dict = new Dictionary<string, string?>(reader.FieldCount);
                    for (var index = 0; index < reader.FieldCount; index++)
                    {
                        var colValue = reader.GetValue(index);
                        dict.Add(
                            uniqueColNames[index],
                            (colValue == null || colValue == DBNull.Value)
                                ? null
                                : colValue is DateTime dateTimeValue
                                    ? dateTimeValue.ToString(reader.GetDataTypeName(index) == "date" ? DEFAULT_DATEONLY_FORMAT : DEFAULT_DATETIME_FORMAT)
                                    : colValue.ToString());
                    }
                    list.Add(dict);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// SELECT文（SqlCommand）の実行結果をDictionaryのList形式で返す。
    /// SELECTのみ（カラムのメタデータ不要）用途のオーバーロード。
    /// </summary>
    /// <param name="cmd">SqlCommandオブジェクト</param>
    public IList<IDictionary<string, string?>> GetRowList(SqlCommand cmd) => GetRowList(cmd, out _);

    /// <summary>
    /// SqlCommandを実行する。
    /// </summary>
    /// <param name="cmd">SqlCommandオブジェクト</param>
    public void ExecuteSqlCommand(SqlCommand cmd)
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
    }

    /// <summary>
    /// 複数のSqlCommandを、同一トランザクション内で実行する。
    /// </summary>
    /// <param name="cmds">SqlCommandオブジェクトのコレクション</param>
    public void ExecuteSqlCommands(IEnumerable<SqlCommand> cmds)
    {
        var callerMethod = new StackFrame(1)?.GetMethod();
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    foreach (var cmd in cmds)
                    {
                        var logMessage = new StringBuilder()
                            .AppendLine($"SQLを実行します。呼び出し元: {callerMethod?.ReflectedType?.FullName}.{callerMethod?.Name}")
                            .Append(GetCommandLogString(cmd))
                            .ToString();
                        Log.ForContext(GetType()).Information(logMessage);
                        cmd.Connection = conn;
                        cmd.Transaction = transaction;
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    Log.ForContext(GetType()).Information("エラーが発生したため、ロールバックします。");
                    transaction.Rollback();
                    throw;
                }
            }
        }
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
