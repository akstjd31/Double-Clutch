using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingStartConfirmPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _confirmText;

    public void Init(int cost)
    {
        _confirmText.text = $"비용 : {cost}\n진행하시겠습니까?";
    }
}
