using UnityEngine;
/// <summary>
/// 역할 : 개인 훈련, 휴식 커맨드에 상속시킬 인터페이스
/// </summary>
public interface ITraining
{
    public string GetNameKey();
    public string GetDescKey();
    
    public void SetTarget(Student target);
    public Student GetTarget();
    public int GetCost();
    public bool IsTeam();
    public void StartAction();
}
