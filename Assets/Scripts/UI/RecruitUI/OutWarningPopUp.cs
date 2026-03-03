using TMPro;
using UnityEngine;

public class OutWarningPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _warningText;

    public void Init(int number)
    {
        _warningText.text = $"선수 {number} 명을 추가로 방출하셔야 합니다.";
    }
}
