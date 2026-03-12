
/// <summary>
/// 팩토리에 있는 데이터를 가지고 원하는 데이터 추출
/// </summary>
public class LeagueManager : Singleton<LeagueManager>
{
    private LeagueFactory _leagueFactory;

    protected override void Awake()
    {
        base.Awake();
        _leagueFactory = this.GetComponent<LeagueFactory>();
    }
}