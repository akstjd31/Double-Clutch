using UnityEngine;

public class IndividualRest : ITraining
{
    Student _target;

    Individual_RestData _data;

    public IndividualRest(Individual_RestData data)
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
        return _data.restCost;
    }

    public bool IsTeam()
    {
        return false;
    }
    
    public void StartAction()
    {
        _target.ChangeCondition(_data.conditionRecovery); //컨디션 회복량만큼 컨디션 수치 회복

        switch (this._target.State)
        {
            case StudentState.None: //건강 상태면 무시
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
            case StudentState.Injured: // 부상 상태면 치료 가능 여부에 따라 치료
                if (_data.isCureInjury == 0)
                {
                    break;
                }
                _target.ChangeState(StudentState.None);
                break;
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
