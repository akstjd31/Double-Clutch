using TMPro;
using UnityEngine;

public class RecruitConfirmPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _confirmText;


    public void Init(int number)
    {
        _confirmText.text = StringManager.Instance.GetFormattedString("UI_Recruit_영입팝업",number);
        StringManager.Instance.ApplyFont(_confirmText);
    }
}
