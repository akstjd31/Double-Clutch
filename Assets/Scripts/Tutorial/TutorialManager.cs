using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private TutorialDataReader _reader;
    [SerializeField] private int[] _chapterStartIndices;    // 인덱스: 챕터, 값: 챕터 별 시작 인덱스

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (_reader == null) return;
        _chapterStartIndices = new int[GetChapterCalculator()];

        for (int i = 0; i < _chapterStartIndices.Length; i++)
            _chapterStartIndices[i] = -1;

        SetChapterStartIndices();
    }

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
    private int GetChapterCalculator()
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

    // 챕터별 시작 인덱스 계산 후 값 집어넣기
    private void SetChapterStartIndices()
    {
        if (_reader == null) return;
        if (_chapterStartIndices == null) return;

        for (int i = 0; i < _chapterStartIndices.Length; i++)
        {
            foreach (var data in _reader.DataList)
            {
                string id = data.tutorialId;
                int idLastNum = int.Parse(id.Substring(id.Length-2));
                
                // 아직 값이 안들어가있다면
                if (_chapterStartIndices[i] == -1)
                {
                    // 같은 그룹중에서
                    if (i == idLastNum - 1)
                    {
                        string slide = data.slideOrder;
                        int slideLastNum = int.Parse(slide.Substring(slide.Length-2));
                        _chapterStartIndices[i] = slideLastNum - 1;
                    }
                }
            }
        }
    }

    public int GetChapterArrayLength()
    {
        if (_chapterStartIndices == null) return -1;
        return _chapterStartIndices.Length;
    }

    // 챕터 번호를 입력하면 해당 챕터의 첫 시작 인덱스를 알려줌
    public int GetChapterStartIndex(int index)
    {
        if (_chapterStartIndices == null) return -1;
        return _chapterStartIndices[index];
    }
}
