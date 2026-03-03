using UnityEngine;

public class MatchPrepState : IState
{
    private readonly StateMachine _sm;
    private readonly GameManager _gm;

    public MatchPrepState(GameManager gm, StateMachine sm)
    {
        _sm = sm;
        _gm = gm;
    }

    public void Enter()
    {
        // 선수 배치 관련 UI 
        // 꺼져 있는 전력 배치창(CharacterList)을 찾아서 활성화
        var charList = Object.FindFirstObjectByType<CharacterList>(FindObjectsInactive.Include);
        if (charList != null)
        {
            charList.gameObject.SetActive(true);
        }
    }

    public void Exit() { }

    public void Update() { }    
}
