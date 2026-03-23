using UnityEngine;

public class GraduationAlbumManager : Singleton<GraduationAlbumManager>
{
    [SerializeField] private GraduationAlbumSaveData _saveData;
    public GraduationAlbumSaveData SaveData => _saveData;
    protected override void Awake()
    {
        base.Awake();
        
    }

    public void Save(GraduationStudent gStd)
    {
        if (_saveData == null)
        {
            _saveData = new GraduationAlbumSaveData();
        }

        _saveData.graduationStudentList.Add(gStd);
        SaveLoadManager.Instance.Save<GraduationAlbumSaveData>(FilePath.GRADUATION_PATH, _saveData);
    }

    public bool HasData()
    {
        if (SaveLoadManager.Instance == null) return false;
        SaveLoadManager.Instance.TryLoad<GraduationAlbumSaveData>(FilePath.GRADUATION_PATH, out _saveData);

        return _saveData != null;
    }

    public GraduationStudent GetGraduationStudentListByIndex(int index)
    {
        if (_saveData == null) return null;
        if (_saveData.graduationStudentList == null) return null;


        foreach (var gList in _saveData.graduationStudentList)
        {
            if (gList.graduatingClass == index)
            {
                if (StudentManager.Instance != null)
                {
                    StudentManager.Instance.InitStudenList(gList.studentList);
                }
                
                return gList;
            }
        }

        return null;
    }
}
