using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InfraUpgradeUI : MonoBehaviour
{
    private Infra _currentInfra = null;
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
        _currentInfra.Upgraded -= UpdateLevelText;
    }

    public void Init(Infra infra)
    {
        _title.text = infra.Name;
        _nameText.text = infra.Name;

        UpdateLevelText(infra.CurrentLevel);

        var valueStr = TextParser.GetKeys(infra.Desc);
        var fommatted = FormatInfraDescText(infra.Desc, "{" + valueStr[0] + "}", infra.GetValueByLevel());
        _effectDescText.text = fommatted;

        _currentInfra = infra;

        if (_upgradeButton == null) return;

        // 최대 레벨에 도달하지 않은 경우에만 활성화
        _upgradeButton.interactable = !infra.IsReachedMaxLevel();

        _currentInfra.Upgraded += UpdateLevelText;
    }
    
    public void UpdateLevelText(int level)
    {
        _levelText.text = level.ToString();
    }

    public void OnClickUpgradeButton()
    {
        if (_currentInfra == null) return;
        _reconfirmUI.gameObject.SetActive(true);

        _reconfirmUI.Init(_currentInfra);
    }

    // 중괄호로 되어있는 부분을 처리 및 전체 문자열을 반환 (이 부분은 UI 스크립트에서 작성해야할듯?)
    private string FormatInfraDescText(string desc, string target, int infraEffectValue) => desc.Replace(target, infraEffectValue.ToString());
}
