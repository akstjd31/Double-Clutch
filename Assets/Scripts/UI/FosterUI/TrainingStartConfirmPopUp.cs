using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingStartConfirmPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _confirmText;
    private int _currentCost;
    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }
    public void Init(int cost)
    {
        _currentCost = cost;
        Refresh();
    }
    private void Refresh()
    {
        _confirmText.text = StringManager.Instance.GetFormattedString("UI_Popup_비용", _currentCost);
        StringManager.Instance.ApplyFont(_confirmText);
    }
}
