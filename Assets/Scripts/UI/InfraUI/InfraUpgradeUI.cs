using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InfraUpgradeUI : MonoBehaviour
{
    private InfraController _currentInfraController = null;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _effectDescText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private ReconfirmUI _reconfirmUI;

    private void Awake()
    {
        
    }

    private void OnDisable()
    {
        _upgradeButton.onClick.RemoveAllListeners();
        _currentInfraController.Upgraded -= UpdateLevelText;
    }

    public void Init(InfraController iController, Infra infra)
    {
        _title.text = infra.name;
        _nameText.text = infra.name;

        UpdateLevelText(infra.currentLevel);

        var valueStr = TextParser.GetKeys(infra.desc);
        var fommatted = FormatInfraDescText(infra.desc, "{" + valueStr[0] + "}", iController.GetCurrentInfraEffectValue());
        _effectDescText.text = fommatted;

        _currentInfraController = iController;

        if (_upgradeButton == null) return;

        // 최대 레벨에 도달하지 않은 경우에만 활성화
        _upgradeButton.interactable = infra.currentLevel < infra.maxLevel;

        _currentInfraController.Upgraded += UpdateLevelText;
    }
    
    public void UpdateLevelText(int level)
    {
        _levelText.text = level.ToString();
    }

    public void OnClickUpgradeButton()
    {
        if (_currentInfraController == null) return;
        _reconfirmUI.gameObject.SetActive(true);

        _reconfirmUI.Init(_currentInfraController);
    }

    // 중괄호로 되어있는 부분을 처리 및 전체 문자열을 반환 (이 부분은 UI 스크립트에서 작성해야할듯?)
    private string FormatInfraDescText(string desc, string target, int infraEffectValue) => desc.Replace(target, infraEffectValue.ToString());
}
