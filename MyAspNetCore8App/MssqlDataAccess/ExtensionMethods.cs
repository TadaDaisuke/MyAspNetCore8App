using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace MyAspNetCore8App.MssqlDataAccess;

/// <summary>
/// SQL Server関連拡張メソッド用クラス
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// StringBuilderの文字列からSqlCommandオブジェクトを生成して返す。
    /// </summary>
    public static SqlCommand ToSqlCommand(this StringBuilder sb)
    {
        return new SqlCommand(sb.ToString());
    }

    /// <summary>
    /// SqlCommandのCommandTimeoutを設定し、SqlCommand自身を返す。
    /// </summary>
    public static SqlCommand SetTimeOut(this SqlCommand cmd, int commandTimeout)
    {
        cmd.CommandTimeout = commandTimeout;
        return cmd;
    }

    /// <summary>
    /// SqlCommandにParameterを追加して値を設定し、SqlCommand自身を返す。
    /// 値にnullが渡された場合は、DBNull.Valueに変換される。
    /// </summary>
    public static SqlCommand AddParameter(this SqlCommand cmd, string parameterName, SqlDbType sqlDbType, object? value)
    {
        cmd.Parameters.Add(parameterName, sqlDbType).Value = value ?? DBNull.Value;
        return cmd;
    }

    /// <summary>
    /// SqlCommandにParameterを追加して値を設定し、SqlCommand自身を返す。
    /// 値にnullが渡された場合は、DBNull.Valueに変換される。
    /// </summary>
    public static SqlCommand AddParameter(this SqlCommand cmd, string parameterName, SqlDbType sqlDbType, int size, object? value)
    {
        cmd.Parameters.Add(parameterName, sqlDbType, size).Value = value ?? DBNull.Value;
        return cmd;
    }

    /// <summary>
    /// SqlCommandに出力用Parameterを追加し、SqlCommand自身を返す。
    /// </summary>
    public static SqlCommand AddOutputParameter(this SqlCommand cmd, string parameterName, SqlDbType sqlDbType, int? size = null)
    {
        if (size == null)
        {
            cmd.Parameters.Add(parameterName, sqlDbType);
        }
        else
        {
            cmd.Parameters.Add(parameterName, sqlDbType, (int)size);
        }
        cmd.Parameters[parameterName].Direction = ParameterDirection.Output;
        return cmd;
    }
}
