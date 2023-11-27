using System.Globalization;

namespace MyAspNetCore8App.Common;

/// <summary>
/// 共通拡張メソッド用クラス
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// 渡された複数のパラメーターのうち1つ以上に一致する場合、trueを返す。
    /// </summary>
    public static bool IsIn<T>(this T source, params T[] values)
        => values.Contains(source);

    /// <summary>
    /// 渡されたコレクションがnullの場合は空のコレクションを、それ以外は渡されたコレクションをそのまま返す。
    /// </summary>
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? values)
        => values ?? Enumerable.Empty<T>();

    /// <summary>
    /// 渡されたコレクション内からnullのアイテムを除いたコレクションを返す。
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> values) where T : class
        => values.Where(v => v != null)!;

    /// <summary>
    /// 渡されたstringがnull/空文字/スペース文字の場合はnullを、それ以外は渡されたstringの値を返す。
    /// </summary>
    public static string? OrNullIfWhiteSpace(this string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value;

    /// <summary>
    /// stringをintに変換した値を返す。
    /// </summary>
    /// <param name="defaultValue">[オプション] 変換できなかった場合に返す値（既定値 = 0）。</param>
    public static int ToInt(this string? s, int defaultValue = 0)
        => int.TryParse(s, out int i) ? i : defaultValue;

    /// <summary>
    /// stringをfloatに変換した値を返す。
    /// </summary>
    /// <param name="defaultValue">[オプション] 変換できなかった場合に返す値（既定値 = 0.0F）。</param>
    public static float ToFloat(this string? s, float defaultValue = 0.0F)
        => float.TryParse(s, out float f) ? f : defaultValue;

    /// <summary>
    /// stringをint（null許容型）に変換した値を返す。変換できなかった場合はnullを返す。
    /// </summary>
    public static int? ToNullableInt(this string? s)
        => int.TryParse(s, out int i) ? i : null;

    /// <summary>
    /// stringをDateOnly（null許容型）に変換した値を返す。変換できなかった場合はnullを返す。
    /// </summary>
    /// <param name="format">[オプション] 日付書式文字列（既定値 = "yyyy-MM-dd"）。</param>
    public static DateOnly? ToNullableDateOnly(this string? s, string? format = null)
        => DateOnly.TryParseExact(s, format ?? DEFAULT_DATEONLY_FORMAT, null, DateTimeStyles.None, out DateOnly d) ? d : null;

    /// <summary>
    /// stringをDateTime（null許容型）に変換した値を返す。変換できなかった場合はnullを返す。
    /// </summary>
    /// <param name="format">[オプション] 日付時刻書式文字列（既定値 = "yyyy-MM-dd HH:mm:ss.fff"）。</param>
    public static DateTime? ToNullableDateTime(this string? s, string? format = null)
        => DateTime.TryParseExact(s, format ?? DEFAULT_DATETIME_FORMAT, null, DateTimeStyles.None, out DateTime d) ? d : null;
}
