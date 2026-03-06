using System.Collections.Generic;
using UnityEngine;

public class StringManager : Singleton<StringManager>
{
    nation _nation;

    [SerializeField] String_TableDataReader _stringDB;
    
    private Dictionary<string, String_TableData> _stringDict = new Dictionary<string, String_TableData>();

    protected override void Awake()
    {
        base.Awake();
        SetLanguage(nation.Kr);
        InitDict();
    }

    private void Start()
    {
        
    }

    private void InitDict()
    {
        if (_stringDB == null)
        {
            Debug.Log("stringDB가 비어있습니다. 인스펙터에서 stringTableDataReader를 할당해주세요.");
            return;
        }

        _stringDict.Clear();
        foreach (var data in _stringDB.DataList)
        {
            if (!_stringDict.ContainsKey(data.stringKey))
            {
                _stringDict.Add(data.stringKey, data);
            }
        }
    }

    public void SetLanguage(nation nation)
    {
        _nation = nation;
    }
    
    
    public string GetString(string key) // 키값을 받아 현재 설정된 언어에 맞는 문자열 반환
    {
        if (_stringDict.TryGetValue(key, out var data))
        {
            switch (_nation)
            {
                case nation.Kr:
                    return data.ko;
                default: //기본 설정은 한국어로
                    return data.ko;
            }
        }

        else
        {
            Debug.LogWarning($"stringKey [{key}]가 stringTable에 없습니다. ");
            return key; //테이블에 없으면 대신 키라도 반환
        }
    }
}