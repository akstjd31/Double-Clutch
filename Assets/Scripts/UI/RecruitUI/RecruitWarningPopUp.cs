using TMPro;
using UnityEngine;

public class RecruitWarningPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _warningText;


    public void Init(int number)
    {
        _warningText.text = $"선수를 {number} 명 더 영입하셔야 합니다.";
    }
}
