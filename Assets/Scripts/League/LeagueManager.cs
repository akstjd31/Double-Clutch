using System;
using UnityEngine;

/// <summary>
/// 팩토리에 있는 데이터를 가지고 원하는 데이터 추출
/// </summary>
public class LeagueManager : Singleton<LeagueManager>
{
    private LeagueFactory _leagueFactory;

    protected override void Awake()
    {
        base.Awake();
        _leagueFactory = this.GetComponent<LeagueFactory>();
    }

    // 위크 ID를 매개변수로 받아 리그 셀렉션 테이블에서 생성 날짜인지 확인
    public bool IsCachingDay(int weekId)
    {
        if (_leagueFactory == null) return false;
        
        var dataList = _leagueFactory.GetTeamSelectionDataList();

        foreach (var data in dataList)
        {
            if (data.weekId.Equals(weekId))
                return true;
        }
        return false;
    }
}