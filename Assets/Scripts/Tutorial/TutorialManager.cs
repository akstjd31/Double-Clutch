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

    // 튜토리얼 챕터별 길이 계산
    public int GetChapterLength()
    {
        if (_reader == null) return -1;
        int max = -1;
        foreach (var data in _reader.DataList)
        {
            string id = data.tutorialId;
            int n = int.Parse(id[id.Length - 1].ToString());
            if (max < n) max = n;
        }

        return max;
    }
}
