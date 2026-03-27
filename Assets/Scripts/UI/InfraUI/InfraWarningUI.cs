using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Game.Constants;

public class InfraWarningUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _needCostText;
    [SerializeField] private Button _confirmButton;

    public void Init(InfraController iController)
    {
        int needCost = iController.GetCostByNextLevel() - GameManager.Instance.SaveData.money;
        _needCostText.text = $"필요한 지원금: {needCost}";
    }

    private void OnEnable()
    {
        if (_confirmButton == null) return;
        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(delegate
        {
            if (AudioManager.Instance == null) return;
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);
            this.gameObject.SetActive(false);
        });
    }
}
