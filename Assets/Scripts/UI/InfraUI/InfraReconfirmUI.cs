using Game.Constants;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class InfraReconfirmUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private Button _okButton;

    private int _cost;
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
        prompt.text = StringManager.Instance.GetFormattedString("UI_Infra_강화확인", _cost == -1 ? "MAX" : _cost.ToString("N0"));
        StringManager.Instance.ApplyFont(prompt);
    }
    public void Init(InfraController iController)
    {
        _cost = iController.GetCostByNextLevel();

        prompt.text = StringManager.Instance.GetFormattedString("UI_Infra_강화확인", _cost == -1 ? "MAX" : _cost.ToString("N0"));
        StringManager.Instance.ApplyFont(prompt);

        if (_okButton == null) return;
        
        _okButton.onClick.RemoveAllListeners();
        _okButton.onClick.AddListener(delegate 
        {
            if (AudioManager.Instance == null) return;
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);

            iController?.Upgrade();
            this.gameObject.SetActive(false);
        });
    }
}
