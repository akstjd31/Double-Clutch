using UnityEngine;
using TMPro;

public class MatchLogItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textTime;
    [SerializeField] private TextMeshProUGUI _textMessage;

    public void Init(MatchLogData log)
    {
        string fullText = log.LogText;

        if (!string.IsNullOrEmpty(fullText) && fullText.Length >= 5)
        {
            _textTime.text = fullText.Substring(0, 5); // "09:59" 시간 추출
            _textMessage.text = fullText.Length > 6 ? fullText.Substring(6).Trim() : ""; // 경기 내용
        }
        else
        {
            _textTime.text = "";
            _textMessage.text = fullText;
        }
    }
}