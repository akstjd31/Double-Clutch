using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Language
{
    Ko,
    En,
    Ja
}


public class StringManager : Singleton<StringManager>
{
    public static event Action OnLanguageChanged;

    private Dictionary<TMP_Text, TMP_FontAsset> _originalFonts = new Dictionary<TMP_Text, TMP_FontAsset>();

    Language _language;
    public Language CurrentLanguage => _language;

    [SerializeField] String_TableDataReader _stringDB;

    [Header("Global Fonts (비워두면 기본 폰트 유지)")]
    [SerializeField] private TMP_FontAsset _koFont;
    [SerializeField] private TMP_FontAsset _enFont;
    [SerializeField] private TMP_FontAsset _jaFont;


    private Dictionary<string, String_TableData> _stringDict = new Dictionary<string, String_TableData>();

    protected override void Awake()
    {
        base.Awake();
        SetLanguage(Language.Ko);
        InitDict();
    }

    private void Start()
    {

    }

    private void InitDict()
    {
        if (_stringDB == null)
        {
            Debug.Log("stringDB가 없습니다. 인스펙터에서 stringTableDataReader를 할당해주세요.");
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

    public void SetLanguage(Language language)
    {
        _language = language;
        Debug.Log($"[StringManager] 언어 변경: {language}");
        OnLanguageChanged?.Invoke();
    }

    public TMP_FontAsset GetFont()
    {
        return _language switch
        {
            Language.En => _enFont,
            Language.Ja => _jaFont,
            _ => _koFont,
        };
    }

    public string GetString(string key)
    {
        if (_stringDict.TryGetValue(key, out var data))
        {
            switch (_language)
            {
                case Language.En:
                    return data.en;
                case Language.Ja:
                    return data.ja;
                case Language.Ko:
                default:
                    return data.ko;
            }
        }
        else
        {
            Debug.LogWarning($"stringKey [{key}]가 stringTable에 없습니다.");
            return key;
        }
    }

    public string GetString(string key, TMP_Text target)
    {
        // 처음 호출 시 원본 폰트 저장
        if (!_originalFonts.ContainsKey(target))
            _originalFonts[target] = target.font;

        string text = GetString(key);
        target.text = text;

        var font = GetFont();
        target.font = font != null ? font : _originalFonts[target]; // null이면 원본 복원
        return text;
    }
}
