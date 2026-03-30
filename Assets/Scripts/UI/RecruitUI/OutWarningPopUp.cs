using TMPro;
using UnityEngine;
using Game.Constants;

public class OutWarningPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _warningText;

    private void OnEnable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.UI_WARNING_01);
    }

    public void Init(int number)
    {
        _warningText.text = StringManager.Instance.GetFormattedString("UI_Release_추가방출팝업",number);
        StringManager.Instance.ApplyFont(_warningText);
    }
}
