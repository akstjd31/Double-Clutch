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

        // 초기화
        foreach (Transform child in _scoreContainer)
        {
            Destroy(child.gameObject);
        }

        // 프리팹 생성
        foreach (var player in players)
        {

            PlayerScoreRow newRow = Instantiate(_scoreRowPrefab, _scoreContainer);

            newRow.Init(player.Position.ToString(), player.Name, player.Score);
        }
    }
}