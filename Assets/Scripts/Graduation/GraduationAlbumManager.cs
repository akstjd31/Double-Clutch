using UnityEngine;

public class GraduationAlbumManager : Singleton<GraduationAlbumManager>
{
    [SerializeField] private GraduationAlbumSaveData _saveData;
    public GraduationAlbumSaveData SaveData => _saveData;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (SaveLoadManager.Instance == null) return;
        SaveLoadManager.Instance.TryLoad<GraduationAlbumSaveData>(FilePath.GRADUATION_PATH, out _saveData);
    }

    public bool HasData() => _saveData != null;
}
