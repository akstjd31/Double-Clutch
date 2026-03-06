using UnityEngine;
using System.Collections.Generic;

public struct MatchPlayerData
{
    public string Position; // 예: "PG", "C"
    public string Name;     // 예: "김민수"
    public int Score;       // 예: 12
}
public class NormalResultPanel : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Transform _scoreContainer; // 아까 만든 Scroll View의 'Content'를 연결
    [SerializeField] private PlayerScoreRow _scoreRowPrefab; // 만들어둔 1줄짜리 프리팹 연결

    // 패널이 열릴 때 호출할 함수
    public void OpenPanel(List<MatchPlayerData> players)
    {
        this.gameObject.SetActive(true);

        // 데이터가 제대로 넘어왔는지 콘솔 창에 확인
        if (players == null)
        {
            Debug.LogError("에러: 넘겨받은 players 리스트가 Null입니다!");
            return;
        }
        Debug.Log($"득점 정보 패널 열림! 전달받은 선수 명단 수: {players.Count}명");

        // 초기화
        foreach (Transform child in _scoreContainer)
        {
            Destroy(child.gameObject);
        }
        _scoreContainer.DetachChildren();
        // 프리팹 생성
        foreach (var player in players)
        {
            PlayerScoreRow newRow = Instantiate(_scoreRowPrefab, _scoreContainer, false);
            newRow.transform.localScale = Vector3.one;

            newRow.Init(player.Position, player.Name, player.Score);
        }
    }
}
