using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Team : MonoBehaviour
{
    //외부에서 세팅해줄 필수 요건
    [SerializeField] Student[] _members = new Student[5];
    [SerializeField] string _teamId;

    //나머지는 데이터에서 조회하기
    Rival_MasterData? _masterData;
    Team_ArchetypeData? _archetypeData;

    //자주쓰는 값(팀 이름) 캐싱
    [SerializeField] string _teamNameKey;
    [SerializeField] Position[] _positions = new Position[5];

    //프로퍼티
    public string TeamNameKey => _teamNameKey;
    public Student[] Members => _members;
    public Position[] Positions => _positions;
    public Rival_MasterData? Rival_MasterData => _masterData;
    public Team_ArchetypeData? Team_ArchetypeData => _archetypeData;

    public void Init(List<Rival_MasterData> masterDb, List<Team_ArchetypeData> archDb) //가지고 있는 팀 아이디 기반으로 데이터 연결(생성 및 로드 직후 호출)
    {
        foreach (var data in masterDb)
        {
            if (_teamId == data.teamId)
            {
                _masterData = data;
            }
        }
        string archId = _masterData.Value.teamArchetypeId;
        foreach (var data in archDb)
        {
            if (archId == data.teamArchetypeId)
            {
                 _archetypeData = data;
            }
        }
        SetupPositionLineup();

        _teamNameKey = _masterData.Value.teamNameKey;
    }

    public Team(string teamId, Student[] members)
    {
        _teamId = teamId;
        _members = members;
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

    //팀 내 모든 선수의 잠재력을 현재 리그에 맞게 재설정
    public void RemakeTeamStat(string leagueLevelId, StudentFactory factory)
    {
        for (int i = 0; i < _members.Length; i++)
        {
            Student student = _members[i];
            Position pos = _positions[i];

            List<Stat> newStat = RivalTeamManager.Instance.GetRivalStatsByLevel(leagueLevelId, this);

            // 2. 학생에게 스탯 주입
            student.SetStat(newStat);
        }

        Debug.Log($"{_teamId} 팀의 스탯이 {leagueLevelId} 레벨에 맞춰 재설정되었습니다.");
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
