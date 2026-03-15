/// <summary>
/// 리그 저장, 로드, 삭제 관련 인터페이스 (일단 만들어놨는데 쓸지는 미정)
/// </summary>
public interface ILeagueSaveService
{
    void Save(LeagueSaveData data);
    LeagueSaveData Load(string leagueId);
    void Delete(string leagueId);
}