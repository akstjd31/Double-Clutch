using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MonthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _monthText;
    [SerializeField] private GameObject[] _weekObjs;
    [SerializeField] List<Image> _lineImg = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> _weekContentList;
    [SerializeField] private bool _isNextMonth;

    public void Init(CalendarManager calMgr)
    {
        if (_weekObjs == null) return;

        if (_lineImg == null)
        {
            foreach (var obj in _weekObjs)
            {
                var img = obj.GetComponentInChildren<Image>(true);
                _lineImg.Add(img);
            }
        }

        var cal = calMgr.GetCalendar();

        _monthText.text = (_isNextMonth ? (cal.month + 1).ToString() : cal.month.ToString()) + StringManager.Instance.GetString("UI_Calendar_월");
        StringManager.Instance.ApplyFont(_monthText);

        // 달에 최대 주차
        int maxWeek = _isNextMonth ? MonthWeekTable.weekCounts[cal.month] : MonthWeekTable.weekCounts[cal.month - 1];
        
        for (int i = 0; i < 5; i++)
        {
            _weekObjs[i].SetActive(i < maxWeek);
            _weekContentList.Add(_weekObjs[i].GetComponentsInChildren<TextMeshProUGUI>()[1]);
        }

        if (GameManager.Instance == null) return;
        var weekId = GameManager.Instance.SaveData.weekId;

        var descList = _isNextMonth ? calMgr.GetDescArrayByNextMonth(weekId) : calMgr.GetDescArrayByMonth(weekId);

        for (int i = 0; i < descList.Count; i++)
        {
            _weekContentList[i].text = descList[i];
            StringManager.Instance.ApplyFont(_weekContentList[i]);
        }

        // 현재 날짜 확인용 밑줄긋기
        if (!_isNextMonth)
        {
            _lineImg[calMgr.GetCalendar().week - 1].gameObject.SetActive(true);
        }
    }
}
