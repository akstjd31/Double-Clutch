using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MonthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _monthText;
    [SerializeField] private GameObject[] _weekObjs;
    [SerializeField] private List<TextMeshProUGUI> _weekContentList;
    [SerializeField] private bool _isNextMonth;

    public void Init(CalendarManager calMgr)
    {
        var cal = calMgr.GetCalendar();

        _monthText.text = (_isNextMonth ? (cal.month + 1).ToString() : cal.month.ToString()) + "월";

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
        }
    }
}
