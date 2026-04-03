using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RecruitConfirmPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _confirmText;

    private int _number;
    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }
    private void OnDisable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }
    private void Refresh()
    {
        _confirmText.text = StringManager.Instance.GetFormattedString("UI_Recruit_영입팝업", _number);
        StringManager.Instance.ApplyFont(_confirmText);
    }
    public void Init(int number)
    {
        _number = number;
        _confirmText.text = StringManager.Instance.GetFormattedString("UI_Recruit_영입팝업",number);
        StringManager.Instance.ApplyFont(_confirmText);
    }
}
