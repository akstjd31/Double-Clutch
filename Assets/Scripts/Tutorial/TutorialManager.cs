using UnityEngine;

public class TutorialManager : MonoBehaviour
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
}
