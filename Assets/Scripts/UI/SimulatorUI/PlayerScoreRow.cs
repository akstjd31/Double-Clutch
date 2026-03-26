using UnityEngine;
using TMPro;

public class PlayerScoreRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textPosition; // 포지션 텍스트 (C, PF, SF 등)
    [SerializeField] private TextMeshProUGUI _textName;     // 선수 이름 텍스트
    [SerializeField] private TextMeshProUGUI _textScore;    // 득점 텍스트

    public void Init(string position, string playerName, int score)
    {
        if (_textPosition != null)
        {
            _textPosition.text = position;
            StringManager.Instance.ApplyFont(_textPosition);
        }

        if (_textName != null)
        {
            _textName.text = playerName;
            StringManager.Instance.ApplyFont(_textName);
        }

        if (_textScore != null)
        {
            _textScore.text = score.ToString();
            StringManager.Instance.ApplyFont(_textScore);
        }
    }
}
