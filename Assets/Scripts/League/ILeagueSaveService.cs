public interface ILeagueSaveService
{
    void Save(LeagueSaveData data);
    LeagueSaveData Load(string leagueId);
    void Delete(string leagueId);
}