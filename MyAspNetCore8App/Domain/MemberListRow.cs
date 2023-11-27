namespace MyAspNetCore8App.Domain;

/// <summary>
/// メンバー一覧の行
/// </summary>
/// <param name="member">メンバー</param>
/// <param name="seq">行番号</param>
/// <param name="totalRecordsCount">総行数</param>
public class MemberListRow(Member member, int seq, int totalRecordsCount)
{
    /// <summary>
    /// メンバー
    /// </summary>
    public Member Member { get; } = member;

    /// <summary>
    /// 行番号
    /// </summary>
    public int Seq { get; } = seq;

    /// <summary>
    /// 総行数
    /// </summary>
    public int TotalRecordsCount { get; } = totalRecordsCount;
}
