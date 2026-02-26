using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamTraining : ITraining
{
    Student _target;
    List<Student> _targets = new List<Student>();

    Team_TrainingData _data;

    public TeamTraining(Team_TrainingData data)
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
        return _data.trainingCost;
    }

    public bool IsTeam()
    {
        return true;
    }

    public void StartAction()
    {
        _targets.Clear();
        _targets.AddRange(StudentManager.Instance.MyStudents);
        foreach (var target in _targets)
        {
            _target = target;

            if (_target.State != StudentState.None)
            {
                continue;
            }

            if (_target.Condition <= 0)
            {
                GetInjuryOrOverwork(_target);
            }

            switch (_data.trainingMode)
            {
                case 1: //일반 훈련 : 모든 스탯을 allGain만큼 증가
                    foreach (potential pot in Enum.GetValues(typeof(potential)))
                    {
                        if (pot == potential.None)
                        {
                            continue;
                        }

                        int growth = _data.allGain + (_data.allGain * (_target.GetStat(pot).GrowthRate / 100));

                        _target.GetStat(pot).GrowAndReturn(growth);
                    }
                    break;
                case 2: //전술 훈련 : 포지션 별 주/부 스탯을 각각 수치만큼 증가
                    potential mainPot = FosterManager.Instance.GetPositionMapping(this._target).mainPotential;
                    int mainGrowth = _data.mainGain + (_data.mainGain * (_target.GetStat(mainPot).GrowthRate / 100));
                    _target.GetStat(mainPot).GrowAndReturn(mainGrowth);

                    potential subPot = FosterManager.Instance.GetPositionMapping(this._target).subPotential;
                    int subGrowth = _data.subGain + (_data.subGain * (_target.GetStat(subPot).GrowthRate / 100));
                    _target.GetStat(subPot).GrowAndReturn(subGrowth);

                    break;
            }

            int conditionCost = UnityEngine.Random.Range(_data.conditionCostMin, _data.conditionCostMax);
            _target.ChangeCondition(-conditionCost);
        }
    }

    private void GetInjuryOrOverwork(Student target)
    {
        int rate = UnityEngine.Random.Range(0, 100);

        if (rate < 60) // 60% 확률 (0~59)
        {
            target.ChangeState(StudentState.OverWorked);
            Debug.Log($"{target.Name} 학생이 과로 상태가 되었습니다.");
        }
        else // 40% 확률 (60~99)
        {
            target.ChangeState(StudentState.Injured);
            Debug.Log($"{target.Name} 학생이 부상 상태가 되었습니다.");
        }
    }

    public string GetNameKey()
    {
        return _data.trainingNameKey;
    }

    public string GetDescKey()
    {
        return _data.trainingdescKey;
    }
}
