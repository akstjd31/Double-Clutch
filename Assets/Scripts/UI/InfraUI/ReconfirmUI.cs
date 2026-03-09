using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReconfirmUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private Button _okButton;

    public void Init(InfraController iController)
    {
        prompt.text = $"강화 시 {iController.GetCostByNextLevel()} 지원금이 사용됩니다.\n강화 하시겠습니까?";

        if (_okButton == null) return;
        _okButton.interactable = iController.HasEnoughUpgradeCost();
        
        _okButton.onClick.AddListener(delegate 
        {
            iController.Upgrade();
            this.gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        _okButton.onClick.RemoveAllListeners();
    }
}
