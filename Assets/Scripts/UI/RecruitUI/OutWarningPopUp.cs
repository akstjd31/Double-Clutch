using TMPro;
using UnityEngine;
using Game.Constants;

public class OutWarningPopUp : MonoBehaviour
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
        Init(_number);
    }

    public void Init(int number)
    {
        _number = number;
        _warningText.text = StringManager.Instance.GetFormattedString("UI_Release_추가방출팝업",number);
        StringManager.Instance.ApplyFont(_warningText);
    }
}
