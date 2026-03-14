using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[Serializable]
public class Team : MonoBehaviour
{
    //외부에서 세팅해줄 필수 요건
    [SerializeField] Student[] _members = new Student[5];
    [SerializeField] string _teamId;
    [SerializeField] int _winCount;
    [SerializeField] int _tieCount;
    [SerializeField] int _LoseCount;

    //나머지는 데이터에서 조회하기
    Rival_MasterData? _masterData;
    Team_ArchetypeData? _archetypeData;

    //자주쓰는 값(팀 이름) 캐싱
    [SerializeField] string _teamNameKey;
    [SerializeField] Position[] _positions = new Position[5];

    //프로퍼티
    public string TeamId => _teamId;
    public string TeamNameKey => _teamNameKey;
    public Student[] Members => _members;
    public Position[] Positions => _positions;
    public Rival_MasterData? Rival_MasterData => _masterData;
    public Team_ArchetypeData? Team_ArchetypeData => _archetypeData;

    public void Init(Rival_MasterData master, Team_ArchetypeData archetype) //가지고 있는 팀 아이디 기반으로 데이터 연결(생성 및 로드 직후 호출)
    {
        _masterData = master;
        _archetypeData = archetype;
        _teamNameKey = master.teamNameKey;

        SetupPositionLineup();
    }

    public Team(string teamId)
    {
        _teamId = teamId;        
    }
    

    public void SetMember(int index, Student student)
    {
        if (index >= Members.Length)
        {
            Debug.Log("멤버 인덱스 설정 오류");
            return;
        }
        _members[index] = student;
    }

    
    public void UpdateTeamStats(List<Stat>[] newStatsForMembers)
    {
        for (int i = 0; i < _members.Length; i++)
        {
            _members[i].SetStat(newStatsForMembers[i]);
        }
    }
    

    public void SetupPositionLineup()
    {
        if (_archetypeData == null)
        {
            Debug.LogError($"{_teamId}의 아키타입 데이터가 없습니다!");
            return;
        }

        var data = _archetypeData.Value;
        int index = 0;
        
        AddPositionToLineup(Position.PG, data.countPG, ref index);
        AddPositionToLineup(Position.SG, data.countSG, ref index);
        AddPositionToLineup(Position.SF, data.countSF, ref index);
        AddPositionToLineup(Position.PF, data.countPF, ref index);
        AddPositionToLineup(Position.C, data.countC, ref index);
        
        while (index < 5)
        {
            _positions[index++] = Position.SF;
        }
    }    
    private void AddPositionToLineup(Position pos, int count, ref int currentIdx)
    {
        for (int i = 0; i < count; i++)
        {
            if (currentIdx < 5)
            {
                _positions[currentIdx++] = pos;
            }
        }
    }
}
