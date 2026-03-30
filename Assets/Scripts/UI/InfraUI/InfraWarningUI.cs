using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Game.Constants;

public class InfraWarningUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _needCostText;
    [SerializeField] private Button _confirmButton;

    int _needCost;    

    public void Init(InfraController iController)
    {
        int needCost = iController.GetCostByNextLevel() - GameManager.Instance.SaveData.money;
        _needCost = needCost;
        _needCostText.text = StringManager.Instance.GetString("UI_Infra_지원금부족2").Replace("{N}", needCost.ToString());
        StringManager.Instance.ApplyFont(_needCostText);
    }

    private void Refresh()
    {
        _needCostText.text = StringManager.Instance.GetString("UI_Infra_지원금부족2").Replace("{N}", _needCost.ToString());
        StringManager.Instance.ApplyFont(_needCostText);
    }

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
        if (_confirmButton == null) return;
        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(delegate
        {
            if (AudioManager.Instance == null) return;
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);
            this.gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }
}
