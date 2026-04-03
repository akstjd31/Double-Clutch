using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Game.Constants;

public class SeasonOutUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _button;
    public void Init()
    {
        if (_text == null) return;
        if (StringManager.Instance == null) return;
        if (!string.IsNullOrWhiteSpace(_text.text)) return;

        _text.text = StringManager.Instance.GetString("Str_시즌아웃");
        StringManager.Instance.ApplyFont(_text);
    }

    // 클릭 시 현재 주차 스킵
    public void OnClickConfirmButton()
    {
        if (CalendarManager.Instance == null) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);
        

        CalendarManager.Instance.NextTurn();
        this.gameObject.SetActive(false);
    }
}
