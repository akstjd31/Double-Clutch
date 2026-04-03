using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SeasonOutUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _button;
    private void OnEnable()
    {
        if (_text == null) return;
        if (StringManager.Instance == null) return;
        if (!string.IsNullOrWhiteSpace(_text.text)) return;

        _text.text = StringManager.Instance.GetString("Str_시즌아웃");

        if (_button == null) return;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClickConfirmButton);
    }

    // 클릭 시 현재 주차 스킵
    public void OnClickConfirmButton()
    {
        if (CalendarManager.Instance == null) return;

        CalendarManager.Instance.NextTurn();
        this.gameObject.SetActive(false);
    }
}
