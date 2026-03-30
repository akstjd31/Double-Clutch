using TMPro;
using UnityEngine;

public class OutConfirmPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _confirmText;


    public void Init(int number)
    {
        _confirmText.text = StringManager.Instance.GetFormattedString("UI_Release_방출팝업", number);
        StringManager.Instance.ApplyFont(_confirmText);
    }
}
