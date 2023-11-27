using Microsoft.Data.SqlClient;

namespace MyAspNetCore8App.Utilities;

/// <summary>
/// Excel生成ユーティリティーインターフェース
/// </summary>
public interface IExcelCreator
{
    /// <summary>
    /// SELECT文（SqlCommand）の実行結果からExcelファイルを生成し、そのバイト配列を返す。
    /// </summary>
    /// <param name="cmd">SqlCommandオブジェクト</param>
    /// <param name="sheetName">ワークシート名</param>
    public byte[] CreateFileBytes(SqlCommand cmd, string sheetName);
}
