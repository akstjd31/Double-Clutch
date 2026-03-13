using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;

public class EventString : MonoBehaviour
{
    [SerializeField] private RandomEventStringDataReader _randomEventStringReader;
    [SerializeField] private Event_ResultDataReader _resultDataReader;



    private Dictionary<string, string> _koScreenPlay = new();
    private Dictionary<string, string> _enScreenPlay = new();
    private Dictionary<string, string> _jaScreenPlay = new();

    private Dictionary<string, List<Event_ResultData>> _resultDataDictionary = new ();
    //private Dictionary<string, Event_DataModel> _dataModelDictionary = new();
    //private Dictionary<string, Event_ChoiceData> _choiceDataDictionary = new();

    public Dictionary<string, string> KoScreenPlay => _koScreenPlay;
    public Dictionary<string, string> EnScreenPlay => _enScreenPlay;
    public Dictionary<string, string> JaScreenPlay => _jaScreenPlay;
    public Dictionary<string, List<Event_ResultData>> ResultData => _resultDataDictionary;
    //public Dictionary<string, Event_DataModel> DataModelDictionary => _dataModelDictionary;
    //public Dictionary<string, Event_ScriptSelectorData> ScriptSelectorDataDictionary => _scriptSelectorDataDictionary;
    //public Dictionary<string, Event_ChoiceData> ChoiceDataDictionary => _choiceDataDictionary;

    private void Start()
    {
        SaveScreenPlayString();
        SaveEvent();
    }

    private void SaveScreenPlayString()
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

    private void SaveEvent()
    {
        var eventData = _resultDataReader.DataList;

        if(_resultDataDictionary.Count < 1)
        {
            for (int i = 0; i < eventData.Count; i++)
            {
                string choiceId = eventData[i].choiceId;

                //ID 키값으로 딕셔너리가 없으면 새로 생성
                if (!_resultDataDictionary.TryGetValue(choiceId, out List<Event_ResultData> List))
                {
                    List = new();
                    _resultDataDictionary.Add(choiceId, List);
                }
                List.Add(eventData[i]);
            }
        }
    }
}
