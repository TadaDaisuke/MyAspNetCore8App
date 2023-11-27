namespace MyAspNetCore8App.Domain;

/// <summary>
/// メンバーサービス実装クラス
/// </summary>
/// <param name="memberRepository">メンバーリポジトリー</param>
public class MemberService(IMemberRepository memberRepository) : IMemberService
{
    /// <inheritdoc/>
    public IEnumerable<MemberListRow> SearchMembers(MemberSearchCondition searchCondition)
        => memberRepository.SearchMembers(searchCondition);

    /// <inheritdoc/>
    public Member? GetMember(string memberCode)
        => memberRepository.GetMember(memberCode);

    /// <inheritdoc/>
    public void SaveMember(Member member)
        => memberRepository.SaveMember(member);

    /// <inheritdoc/>
    public byte[] DownloadMembers(MemberSearchCondition searchCondition)
        => memberRepository.CreateExcelBytes(searchCondition, "Members");

    /// <inheritdoc/>
    public IEnumerable<string> SuggestMemberCode(string memberCodePart)
        => memberRepository.SuggestMemberCode(memberCodePart);
}
