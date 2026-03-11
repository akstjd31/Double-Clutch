using System.Collections.Generic;
using UnityEngine;

public class EventString : MonoBehaviour
{
    [SerializeField] private String_RandomEventStringReader _randomEventStringReader;

    private Dictionary<string, string> _koScreenPlay = new();
    private Dictionary<string, string> _enScreenPlay = new();
    private Dictionary<string, string> _jaScreenPlay = new();

    public Dictionary<string, string> KoScreenPlay => _koScreenPlay;
    public Dictionary<string, string> EnScreenPlay => _enScreenPlay;
    public Dictionary<string, string> JaScreenPlay => _jaScreenPlay;

    private void Start()
    {
        SaveString();
    }

    private void SaveString()
    {
        var stringData = _randomEventStringReader.DataList;
        
        if(_koScreenPlay.Count < 1)
        {
            for (int i = 0; i < stringData.Count; i++)
            {
                _koScreenPlay.Add(stringData[i].stringKey, stringData[i].ko);
            }
        }

        if(_enScreenPlay.Count < 1)
        {
            for (int i = 0; i < stringData.Count; i++)
            {
                _enScreenPlay.Add(stringData[i].stringKey, stringData[i].en);
            }
        }

        if(_jaScreenPlay.Count < 1)
        {
            for (int i = 0; i < stringData.Count; i++)
            {
                _jaScreenPlay.Add(stringData[i].stringKey, stringData[i].ja);
            }
        }
    }
}
