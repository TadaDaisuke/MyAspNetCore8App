namespace MyAspNetCore8App.MssqlDataAccess;

/// <summary>
/// カラムのメタデータ
/// </summary>
/// <param name="columnName">カラム名</param>
/// <param name="dataTypeName">データ型名</param>
/// <param name="numericScale">数値の小数点以下桁数</param>
public class ColumnSchema(string columnName, string dataTypeName, short numericScale)
{
    /// <summary>
    /// カラム名
    /// </summary>
    public string ColumnName { get; } = columnName;

    /// <summary>
    /// データ型名
    /// </summary>
    public string DataTypeName { get; } = dataTypeName;

    /// <summary>
    /// 数値の小数点以下桁数
    /// </summary>
    public short NumericScale { get; } = numericScale;
}
