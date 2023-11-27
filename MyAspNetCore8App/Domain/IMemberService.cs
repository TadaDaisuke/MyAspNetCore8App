namespace MyAspNetCore8App.Domain;

/// <summary>
/// メンバーサービスインターフェース
/// </summary>
public interface IMemberService
{
    /// <summary>
    /// 検索条件に合致するメンバー（一覧の行のコレクション）を返す。
    /// </summary>
    public IEnumerable<MemberListRow> SearchMembers(MemberSearchCondition searchCondition);

    /// <summary>
    /// 該当するメンバーコードのメンバーを返す。
    /// </summary>
    public Member? GetMember(string memberCode);

    /// <summary>
    /// メンバー情報を登録/更新する。
    /// </summary>
    public void SaveMember(Member member);

    /// <summary>
    /// 検索条件に合致するメンバー一覧のExcelファイルを生成し、そのバイト配列を返す。
    /// </summary>
    public byte[] DownloadMembers(MemberSearchCondition searchCondition);

    /// <summary>
    /// 渡された文字列が中間一致するメンバーコードのコレクション（最大20件）を返す。
    /// </summary>
    public IEnumerable<string> SuggestMemberCode(string memberCodePart);
}
