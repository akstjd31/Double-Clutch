using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private TutorialDataReader _reader;
    
    public TutorialData? GetData(int idx)
    {
        if (_reader == null) return null;
        if (_reader.DataList.Count <= idx) return null;
        return _reader.DataList[idx];
    }

    public int GetDataListLength()
    {
        if (_reader == null) return -1;
        return _reader.DataList.Count;
    }

    // 해당 튜토리얼이 몇 개로 나눠지는지?
    // public int GetStepLength()
    // {
    //     if (_reader == null) return -1;
    //     int max = 1;
    //     foreach (var data in _reader.DataList)
    //     {
    //     }
    // }
}
