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

        _nextMonthUI.gameObject.SetActive(_calMgr.GetCalendar().month != 2);
        if (_nextMonthUI.gameObject.activeSelf)
            _nextMonthUI.Init(_calMgr);
    }
}
