using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalendarUI : MonoBehaviour
{
    private CalendarManager _calMgr;
    [SerializeField] private MonthUI _thisMonthUI;
    [SerializeField] private MonthUI _nextMonthUI;
    private void OnEnable()
    {
        if (_calMgr == null)
            _calMgr = GameObject.FindAnyObjectByType<CalendarManager>();

        if (_thisMonthUI == null) return;
        if (_nextMonthUI == null) return;

        _thisMonthUI.gameObject.SetActive(true);
        _thisMonthUI.Init(_calMgr);

        if (_calMgr.GetCalendar().month != 1)
        {
            _nextMonthUI.gameObject.SetActive(true);
            _nextMonthUI.Init(_calMgr);
        }
    }
}
