using TMPro;
using UnityEngine;

public class RecruitConfirmPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _confirmText;


    public void Init(int number)
    {
        _confirmText.text = $"선수를 {number} 명 영입하시겠습니까?";
    }
}
