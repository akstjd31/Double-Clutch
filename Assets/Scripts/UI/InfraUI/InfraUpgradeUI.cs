using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;


public class InfraUpgradeUI : MonoBehaviour
{
    private InfraController _controller;
    private Infra _infra;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _costText;

    [SerializeField] private TextMeshProUGUI _effectDescText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private InfraReconfirmUI _reconfirmUI;
    [SerializeField] private InfraWarningUI _warningUI;

    [Header("업그레이드 배경리소스")]
    [SerializeField] private Image _backGroundImage;
    [SerializeField] private Sprite _clinic;
    [SerializeField] private Sprite _dormitory;
    [SerializeField] private Sprite _frontOffice;
    [SerializeField] private Sprite _analyticsRoom;
    [SerializeField] private Sprite _conditioningGym;


    private void Awake()
    {
        
    }
    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }
    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
        _upgradeButton.onClick.RemoveAllListeners();
        if (_controller != null)
            _controller.Upgraded -= UpdateLevelText;
    }

    public void Init(InfraController controller, Infra infra)
    {
        if (_controller != null)
            _controller.Upgraded -= UpdateLevelText;

        _controller = controller;
        _infra = infra;

        _upgradeButton.interactable = !(_infra.currentLevel >= _infra.maxLevel);
        _controller.Upgraded += UpdateLevelText;

        SetBackGroundImage(infra.infraEffectType);

        Refresh();
    }
    private void Refresh()
    {
        if (_infra == null) return;
        if (StringManager.Instance == null) return;

        StringManager.Instance.GetString(_infra.nameKey, _title);
        StringManager.Instance.GetString(_infra.nameKey, _nameText);

        _levelText.text = _infra.currentLevel.ToString();
        _costText.text = _controller.GetCostByNextLevel().ToString();

        string originDesc = StringManager.Instance.GetString(_infra.descKey);
        var keys = TextParser.GetKeys(originDesc);
        if (keys != null && keys.Count > 0)
        {
            int value = _controller.GetCurrentInfraEffectValue();
            _effectDescText.text = originDesc.Replace("{" + keys[0] + "}", value.ToString());
        }
        else
        {
            _effectDescText.text = originDesc;
        }
        StringManager.Instance.ApplyFont(_effectDescText);
    }

    public void UpdateLevelText(int level)
    {
        _upgradeButton.interactable = !(_infra.currentLevel >= _infra.maxLevel);
        Refresh();
    }

    public void OnClickUpgradeButton()
    {
        if (_controller == null) return;

        if (_controller.HasEnoughUpgradeCost())
        {
            _reconfirmUI.gameObject.SetActive(true);
            _reconfirmUI.Init(_controller);
        }
        else

        {
            _warningUI.gameObject.SetActive(true);
            _warningUI.Init(_controller);
        }
    }

    // 중괄호로 되어있는 부분을 처리 및 전체 문자열을 반환 (이 부분은 UI 스크립트에서 작성해야할듯?)
    private string FormatInfraDescText(string desc, string target, int infraEffectValue) => desc.Replace(target, infraEffectValue.ToString());

    private void SetBackGroundImage(infraEffectType type)
    {
        Sprite background = null;
        switch (type)
        {
            case infraEffectType.RestCostDiscount:
                background = _clinic;
                break;
            case infraEffectType.AddRoster:
                background = _dormitory;
                break;
            case infraEffectType.RewardGoldBonus:
                background = _frontOffice;
                break;
            case infraEffectType.AddTactic:
                background = _analyticsRoom;
                break;
            case infraEffectType.TrainingBonus:
                background = _conditioningGym;
                break;
            default:
                background = _frontOffice;
                break;
        }

        _backGroundImage.sprite = background;
    }
}
