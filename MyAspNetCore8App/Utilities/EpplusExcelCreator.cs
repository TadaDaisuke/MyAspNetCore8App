﻿using Microsoft.Data.SqlClient;
using MyAspNetCore8App.MssqlDataAccess;
using OfficeOpenXml;
using System.Data;
using System.Drawing;

namespace MyAspNetCore8App.Utilities;

/// <summary>
/// EPPlus を使用した、Excel生成ユーティリティー実装クラス
/// </summary>
/// <param name="context">MyDatabaseアクセス関連のコンテキスト</param>
/// <param name="excelSettings">Excel出力用設定</param>
public class EpplusExcelCreator(MyDatabaseContext context, ExcelSettings excelSettings) : IExcelCreator
{
    /// <inheritdoc/>
    public byte[] CreateFileBytes(SqlCommand cmd, string sheetName)
    {
        byte[] bytes;
        using (var stream = new MemoryStream())
        {
            using (var package = new ExcelPackage(stream))
            {
                OutputExcelSheet(package, cmd, sheetName);
                package.Save();
            }
            bytes = stream.ToArray();
        }
        return bytes;
    }

    /// <summary>
    /// SELECT文（SqlCommand）の実行結果をワークシートに出力する
    /// </summary>
    /// <param name="package">ExcelPackageオブジェクト</param>
    /// <param name="cmd">SqlCommandオブジェクト</param>
    /// <param name="sheetName">ワークシート名</param>
    private void OutputExcelSheet(ExcelPackage package, SqlCommand cmd, string sheetName)
    {
        const int MAX_ROW = 1048575;
        // シートの生成
        var sheet = AddWorksheet(package.Workbook, sheetName);
        // クエリの実行
        List<Dictionary<string, string?>> rowList;
        DataTable schemaTable;
        try
        {
            rowList = context.GetRowList(cmd, out schemaTable);
        }
        catch (Exception ex)
        {
            if (ex is OutOfMemoryException)
            {
                sheet.Cells[1, 1].Value = "メモリ不足のエラーが発生しました。";
            }
            else if (ex is SqlException)
            {
                sheet.Cells[1, 1].Value = "データベースのエラーが発生しました。";
            }
            else
            {
                sheet.Cells[1, 1].Value = "エラーが発生しました。";
            }
            return;
        }
        var currentRow = 2;
        foreach (var row in rowList)
        {
            if (currentRow > MAX_ROW + 1)
            {
                AdjustSheet(sheet, schemaTable);
                sheet = AddWorksheet(package.Workbook);
                currentRow = 2;
            }
            if (currentRow == 2)
            {
                for (var columnIndex = 1; columnIndex <= schemaTable.Rows.Count; columnIndex++)
                {
                    // 列毎の書式設定
                    switch (schemaTable.Rows[columnIndex - 1].Field<string>("DataTypeName"))
                    {
                        case "char":
                        case "varchar":
                        case "nchar":
                        case "nvarchar":
                        case "binary":
                        case "varbinary":
                            sheet.Column(columnIndex).Style.Numberformat.Format = "@";
                            break;
                        case "date":
                            sheet.Column(columnIndex).Style.Numberformat.Format = "yyyy-MM-dd";
                            break;
                        case "datetime":
                            sheet.Column(columnIndex).Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                            break;
                        case "int":
                        case "bigint":
                        case "smallint":
                        case "tinyint":
                            sheet.Column(columnIndex).Style.Numberformat.Format = "0";
                            break;
                        case "decimal":
                            var decimalFormat = "#,##0";
                            var scale = schemaTable.Rows[columnIndex - 1].Field<short>("NumericScale");
                            if (scale > 0)
                            {
                                decimalFormat += $".{new string('0', scale)}";
                            }
                            sheet.Column(columnIndex).Style.Numberformat.Format = decimalFormat;
                            break;
                    }
                    // タイトル行の設定
                    sheet.Cells[1, columnIndex].Style.Numberformat.Format = "@";
                    sheet.Cells[1, columnIndex].Value = schemaTable.Rows[columnIndex - 1].Field<string>("ColumnName");
                    sheet.Cells[1, columnIndex].Style.TextRotation = 180;
                }
            }
            var currentColumn = 1;
            foreach (var v in row.Values)
            {
                SetCellValue(sheet.Cells[currentRow, currentColumn], v, schemaTable.Rows[currentColumn - 1].Field<string>("DataTypeName"));
                currentColumn++;
            }
            currentRow++;
        }
        AdjustSheet(sheet, schemaTable);
    }

    /// <summary>
    /// Excelのセルに値をセットする
    /// </summary>
    /// <param name="cell">セル</param>
    /// <param name="value">値</param>
    /// <param name="dataTypeName">データ型</param>
    private static void SetCellValue(ExcelRange cell, string? value, string? dataTypeName)
    {
        switch (dataTypeName)
        {
            case "int":
            case "smallint":
            case "tinyint":
                if (int.TryParse(value, out var workInt))
                {
                    cell.Value = workInt;
                    return;
                }
                break;
            case "bigint":
                if (long.TryParse(value, out var workLong))
                {
                    cell.Value = workLong;
                    return;
                }
                break;
            case "decimal":
                if (decimal.TryParse(value, out var workDecimal))
                {
                    cell.Value = workDecimal;
                    return;
                }
                break;
            case "real":
                if (float.TryParse(value, out var workFloat))
                {
                    cell.Value = workFloat;
                    return;
                }
                break;
            case "float":
                if (double.TryParse(value, out var workDouble))
                {
                    cell.Value = workDouble;
                    return;
                }
                break;
            case "bit":
                if (bool.TryParse(value, out var workBool))
                {
                    cell.Value = workBool;
                    return;
                }
                break;
            case "date":
            case "datetime":
                if (DateTime.TryParse(value, out var workDateTime))
                {
                    var oaDate = workDateTime.ToOADate();
                    cell.Value = (oaDate < 60) ? oaDate - 1 : oaDate; // Excelで1900年2月28日以前の日付が一日ずれる問題に対応
                    return;
                }
                break;
        }
        if (value?.Length > 32767)
        {
            cell.Value = "(Excelに出力可能なデータ長を超えています)";
            return;
        }
        cell.Value = value;
    }

    /// <summary>
    /// シート名が重複しないようにシートを追加する
    /// </summary>
    /// <param name="workbook">ExcelWorkbookオブジェクト</param>
    /// <param name="sheetName">シート名（省略可）</param>
    /// <returns>追加されたExcelWorksheetオブジェクト</returns>
    private ExcelWorksheet AddWorksheet(ExcelWorkbook workbook, string? sheetName = null)
    {
        var newSheetName = string.IsNullOrWhiteSpace(sheetName)
            ? "Sheet1"
            : (sheetName.Length > 31)
                ? sheetName[..31]
                : sheetName;
        for (var i = 2; i <= 100; i++)
        {
            if (workbook.Worksheets.OfType<ExcelWorksheet>().Any(x => x.Name == newSheetName))
            {
                newSheetName = $"Sheet{i}";
            }
            else
            {
                var sheet = workbook.Worksheets.Add(newSheetName);
                var fontName = excelSettings.FontName;
                var fontSize = excelSettings.FontSize.ToFloat(10F);
                sheet.Cells.Style.Font.SetFromFont(new Font(fontName, fontSize));
                return sheet;
            }
        }
        return workbook.Worksheets.Add(DateTime.Now.ToString("yyyyMMddHHmmssfff"));
    }

    /// <summary>
    /// 入力済みシートの表示を調整する
    /// </summary>
    /// <param name="sheet">ExcelWorksheetオブジェクト</param>
    /// <param name="schemaTable">列情報のDataTable</param>
    private void AdjustSheet(ExcelWorksheet sheet, DataTable schemaTable)
    {
        sheet.View.FreezePanes(2, 1);
        for (var columnIndex = 1; columnIndex <= schemaTable.Rows.Count; columnIndex++)
        {
            if (schemaTable.Rows[columnIndex - 1].Field<string>("DataTypeName") == "bit")
            {
                var fontSize = excelSettings.FontSize.ToFloat(10F);
                sheet.Column(columnIndex).Width = 0.7 * fontSize;
            }
            else
            {
                sheet.Column(columnIndex).AutoFit(3, 120);
            }
        }
    }
}
