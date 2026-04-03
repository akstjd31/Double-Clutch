using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MonthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _monthText;
    [SerializeField] private GameObject[] _weekObjs;
    [SerializeField] private List<Image> _lineImg = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> _weekContentList = new List<TextMeshProUGUI>();
    [SerializeField] private bool _isNextMonth;

    public void Init(CalendarManager calMgr)
    {
        if (_weekObjs == null || _weekObjs.Length == 0) return;
        if (calMgr == null) return;

        if (_lineImg == null || _lineImg.Count == 0)
        {
            _lineImg = new List<Image>();

            foreach (var obj in _weekObjs)
            {
                var img = obj.GetComponentInChildren<Image>(true);
                _lineImg.Add(img);
            }
        }

        var cal = calMgr.GetCalendar();

        int nextMonth = cal.month + 1;
        if (nextMonth > 12) nextMonth = 1;

        int displayMonth = _isNextMonth ? nextMonth : cal.month;

        _monthText.text = displayMonth.ToString() + StringManager.Instance.GetString("UI_Calendar_월");
        StringManager.Instance.ApplyFont(_monthText);

        int maxWeek = MonthWeekTable.weekCounts[displayMonth - 1];

        _weekContentList.Clear();

        for (int i = 0; i < _weekObjs.Length; i++)
        {
            bool active = i < maxWeek;
            _weekObjs[i].SetActive(active);

            var texts = _weekObjs[i].GetComponentsInChildren<TextMeshProUGUI>(true);
            if (texts.Length > 1)
                _weekContentList.Add(texts[1]);
            else
                _weekContentList.Add(null);
        }

        if (GameManager.Instance == null) return;

        int weekId = GameManager.Instance.SaveData.weekId;
        var descList = _isNextMonth
            ? calMgr.GetDescArrayByNextMonth(weekId)
            : calMgr.GetDescArrayByMonth(weekId);

        for (int i = 0; i < descList.Count && i < _weekContentList.Count; i++)
        {
            if (_weekContentList[i] == null) continue;

            _weekContentList[i].text = descList[i];
            StringManager.Instance.ApplyFont(_weekContentList[i]);
        }

        foreach (var img in _lineImg)
        {
            if (img != null)
                img.gameObject.SetActive(false);
        }

        if (!_isNextMonth)
        {
            int currentWeekIndex = cal.week - 1;
            if (currentWeekIndex >= 0 && currentWeekIndex < _lineImg.Count && _lineImg[currentWeekIndex] != null)
            {
                _lineImg[currentWeekIndex].gameObject.SetActive(true);
            }
        }
    }
}