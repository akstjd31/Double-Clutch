using System;
using UnityEngine;

public class CalendarManager : Singleton<CalendarManager>
{
    private const int MAX_WEEK = 4;
    public event Action OnWeekChanged;


    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        NextTurn();
    }

    private void NextTurn()
    {
        var week = CalendarSaveData._week;
        var month = CalendarSaveData._month;
        
        if (week >= MAX_WEEK)
        {
            month++;
            week = 1;
        }
        else
        {
            week++;
        }

        CalendarSaveData._week = week;
        CalendarSaveData._month = month;
        
        OnWeekChanged?.Invoke();
    }
}

public static class CalendarSaveData
{
    private const string KEY_YEAR = "YEAR";
    private const string KEY_MONTH = "MONTH";
    private const string KEY_WEEK = "WEEK";

    public static int _year
    {
        get => PlayerPrefs.GetInt(KEY_YEAR, 0);
        set
        {
            PlayerPrefs.SetInt(KEY_YEAR, value);
            PlayerPrefs.Save();
        }
    }

    public static int _month
    {
        get => PlayerPrefs.GetInt(KEY_MONTH, 1);
        set
        {
            PlayerPrefs.SetInt(KEY_MONTH, value);
            PlayerPrefs.Save();
        }
    }

    public static int _week
    {
        get => PlayerPrefs.GetInt(KEY_WEEK, 0);
        set
        {
            PlayerPrefs.SetInt(KEY_WEEK, value);
            PlayerPrefs.Save();
        }
    }
}
