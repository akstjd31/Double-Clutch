using UnityEngine;
using TMPro;

public class InfraWarningUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _needCostText;

    public void Init(InfraController iController)
    {
        int needCost = iController.GetCostByNextLevel() - GameManager.Instance.SaveData.money;
        _needCostText.text = $"필요한 지원금: {needCost}";
    }
}
