using Game.Constants;
using TMPro;
using UnityEngine;

public class OutConfirmPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _confirmText;
    private int _number;
    private void OnEnable()
    {
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
        _confirmText.text = StringManager.Instance.GetFormattedString("UI_Release_방출팝업", number);
        StringManager.Instance.ApplyFont(_confirmText);
    }
}
