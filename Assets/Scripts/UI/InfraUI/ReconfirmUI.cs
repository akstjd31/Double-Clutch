using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReconfirmUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private Button _okButton;

    public void Init(Infra infra)
    {
        prompt.text = $"강화 시 {infra.GetCostByNextLevel()} 지원금이 사용됩니다.\n강화 하시겠습니까?";

        if (_okButton == null) return;
        _okButton.interactable = infra.HasEnoughUpgradeCost();
        
        _okButton.onClick.AddListener(delegate 
        {
            infra.Upgrade();
            this.gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        _okButton.onClick.RemoveAllListeners();
    }
}
