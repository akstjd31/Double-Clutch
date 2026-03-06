using TMPro;
using UnityEngine;

public class OutConfirmPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _confirmText;


    public void Init(int number)
    {
        _confirmText.text = $"선수를 {number} 명 방출하시겠습니까?";
    }
}
