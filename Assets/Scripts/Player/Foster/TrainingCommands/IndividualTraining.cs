using UnityEngine;

public class IndividualTraining : ITraining
{
    Student _target;
    
    Individual_TrainingData _data;

    public IndividualTraining(Individual_TrainingData data)
    {        
        _data = data;
    }

    public void SetTarget(Student target)
    {
        _target = target;
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
        return false;
    }

    public void StartAction()
    {
        if (_target.Condition <= 0)
        {
            GetInjuryOrOverwork(_target);
        }

        int mainGrowth = _data.mainGain + (_data.mainGain * (_target.GetStat(_data.mainPotential).GrowthRate + InfraManager.Instance.GetInfraEffectValueByEffectType(infraEffectType.TrainingBonus) //패시브 스킬
            / 100)); //현재는 성장률만 반영.            
        
        _target.GetStat(_data.mainPotential).GrowAndReturn(mainGrowth);
        _target.AddChangedPotential(_data.mainPotential);
        if (_data.subPotential != potential.None) //부 잠재력이 설정된 훈련이라면
        {
            int subGrowth = _data.subGain + (_data.subGain * (_target.GetStat(_data.subPotential).GrowthRate // + 시설 효율 + 패시브 스킬
            / 100)); //현재는 성장률만 반영.
            _target.GetStat(_data.subPotential).GrowAndReturn(subGrowth);
            _target.AddChangedPotential(_data.subPotential);
        }

        _target.ChangeCondition(-(Random.Range(_data.conditionCostMin, _data.conditionCostMax)));
        _target.OnStatChanged();
        _target.ResetTrainingSchedule();

    }
    private void GetInjuryOrOverwork(Student target)
    {
        //target.ChangeState(StudentState.OverWorked);
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
