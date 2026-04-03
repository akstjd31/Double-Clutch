using Game.Constants;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RecruitWarningPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _warningText;

    private int _number;
    private void OnEnable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.UI_WARNING_01);
        StringManager.OnLanguageChanged += Refresh;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }

    private void Refresh()
    {
        _warningText.text = StringManager.Instance.GetFormattedString("UI_Recruit_부족팝업", _number);
        StringManager.Instance.ApplyFont(_warningText);
    }
    public void Init(int number)
    {
        _number = number;
        _warningText.text = StringManager.Instance.GetFormattedString("UI_Recruit_부족팝업",number);
        StringManager.Instance.ApplyFont(_warningText);
    }
}
