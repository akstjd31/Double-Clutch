using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private TutorialDataReader _reader;

    // key: tutorialId, value: 시작 인덱스
    [SerializeField] private SerializedDictionary<string, int> _chapterStartIndexData;

    // key: tutorialId, value: 끝 인덱스
    [SerializeField] private SerializedDictionary<string, int> _chapterEndIndexData;

    public event Action<string> OnStartTutorial;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (_reader == null || _reader.DataList == null || _reader.DataList.Count == 0)
            return;

        _chapterStartIndexData = new SerializedDictionary<string, int>();
        _chapterEndIndexData = new SerializedDictionary<string, int>();

        SetChapterRangeData();
    }

    public void StartTutorial(string id)
    {
        if (_chapterStartIndexData == null) return;
        if (!_chapterStartIndexData.ContainsKey(id)) return;

        OnStartTutorial?.Invoke(id);
        Debug.Log($"튜토리얼 시작! id:{id}");
    }

    public List<TutorialData> GetData(string id)
    {
        if (_reader == null || _reader.DataList == null) return null;
        if (_chapterStartIndexData == null || _chapterEndIndexData == null) return null;
        if (!_chapterStartIndexData.ContainsKey(id) || !_chapterEndIndexData.ContainsKey(id)) return null;

        int start = _chapterStartIndexData[id];
        int end = _chapterEndIndexData[id];

        var result = new List<TutorialData>();

        for (int i = start; i <= end; i++)
        {
            result.Add(_reader.DataList[i]);
        }

        return result;
    }

    private void SetChapterRangeData()
    {
        if (_reader == null || _reader.DataList == null) return;

        var dataList = _reader.DataList;

        for (int i = 0; i < dataList.Count; i++)
        {
            string tutorialId = dataList[i].tutorialId;

            // 시작 인덱스 최초 1회 저장
            if (!_chapterStartIndexData.ContainsKey(tutorialId))
            {
                _chapterStartIndexData[tutorialId] = i;
            }

            // 끝 인덱스는 매번 갱신하면 마지막 값이 남음
            _chapterEndIndexData[tutorialId] = i;
        }
    }

    public int GetDataListLength()
    {
        if (_reader == null || _reader.DataList == null) return -1;
        return _reader.DataList.Count;
    }

    public int GetChapterArrayLength()
    {
        if (_chapterStartIndexData == null) return -1;
        return _chapterStartIndexData.Count;
    }

    public int GetChapterStartIndex(string id)
    {
        if (_chapterStartIndexData == null) return -1;
        if (!_chapterStartIndexData.ContainsKey(id)) return -1;

        return _chapterStartIndexData[id];
    }

    public int GetChapterEndIndex(string id)
    {
        if (_chapterEndIndexData == null) return -1;
        if (!_chapterEndIndexData.ContainsKey(id)) return -1;

        return _chapterEndIndexData[id];
    }
}