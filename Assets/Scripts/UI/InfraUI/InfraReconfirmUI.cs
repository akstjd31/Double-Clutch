using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Game.Constants;

public class InfraReconfirmUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private Button _okButton;

    public void Init(InfraController iController)
    {
        prompt.text = $"강화 시 {iController.GetCostByNextLevel()} 지원금이 사용됩니다.\n강화 하시겠습니까?";

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
