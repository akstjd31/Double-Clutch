using System.Collections.Generic;
using UnityEngine;

public class Team_Rest : ITraining
{
    Student _target;
    List<Student> _targets = new List<Student>();

    Team_RestData _data;

    public Team_Rest(Team_RestData data)
    {        
        _data = data;
    }

    public void SetTarget(Student target)
    {
        if (!_targets.Contains(target))
        {
            _targets.Add(target);
        }
    }

    public Student GetTarget()
    {
        return _target;
    }

    public int GetCost()
    {
        return _data.restCost;
    }

    public bool IsTeam()
    {
        return true;
    }

    public void StartAction() //과로, 부상 개념 확인 후 재정비 요망
    {
        _targets.Clear();
        _targets.AddRange(StudentManager.Instance.MyStudents);
        foreach (var target in _targets)
        {
            _target = target;
            _target.ChangeCondition(_data.conditionRecovery); //컨디션 회복량만큼 컨디션 수치 회복

            switch (this._target.State)
            {
                case StudentState.None: //건강 상태면 무시
                case StudentState.Injured: // 부상 상태는 팀 휴식으로 치료 불가
                    break;
                case StudentState.OverWorked: //과로 상태면 확률에 따라 치료
                    if (_target.CureCount >= 2)
                    {
                        _target.ChangeState(StudentState.None);                        
                        break;
                    }
                    int rate = Random.Range(0, 100);
                    _target.SetCureCount(_target.CureCount + 1);
                    if (rate > _data.cureOverworkProb)
                    {
                        break;
                    }
                    _target.ChangeState(StudentState.None);                    
                    break;
            }
            _target.ResetTrainingSchedule();
        }
    }

    public string GetNameKey()
    {
        return _data.restNameKey;
    }

    public string GetDescKey()
    {
        return _data.restdescKey;
    }
}
