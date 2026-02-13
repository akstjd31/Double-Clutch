/// <summary>
/// 커맨드가 접근할 공용 클래스
/// </summary>
public class GameContext
{
    public PlayerSaveData Save { get; }
    public SaveLoadManager SaveLoadMr { get; }
    public string SaveFileName { get; }

    public GameContext(PlayerSaveData save, SaveLoadManager saveLoadMr, string saveFileName)
    {
        Save = save;
        SaveLoadMr = saveLoadMr;
        SaveFileName = saveFileName;
    }

    public void CommitSave()
    {
        SaveLoadMr.Save(SaveFileName, Save);
    }
}