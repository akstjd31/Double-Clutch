using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfraUpgradeUI : MonoBehaviour
{
    private InfraController _controller;
    private Infra _infra;

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _effectDescText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private ReconfirmUI _reconfirmUI;
    [SerializeField] private InfraNoCostPopUp _warningUI;

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

        Refresh();
    }

    private void Refresh()
    {
        //if (_infra == null) return;
        //if (StringManager.Instance == null) return;

        ////StringManager.Instance.GetString(_infra.nameKey, _title);
        ////StringManager.Instance.GetString(_infra.nameKey, _nameText);

        //_levelText.text = _infra.currentLevel.ToString();

        ////string originDesc = StringManager.Instance.GetString(_infra.descKey);
        //var keys = TextParser.GetKeys(originDesc);
        //if (keys != null && keys.Count > 0)
        //{
        //    int value = _controller.GetCurrentInfraEffectValue();
        //    _effectDescText.text = originDesc.Replace("{" + keys[0] + "}", value.ToString());
        //}
        //else
        //{
        //    _effectDescText.text = originDesc;
        //}
        //StringManager.Instance.ApplyFont(_effectDescText);
    }

    public void UpdateLevelText(int level)
    {
        _levelText.text = level.ToString();
    }

    public void OnClickUpgradeButton()
    {

        if (_controller == null) return;

        if(_controller.HasEnoughUpgradeCost())
        {
            _reconfirmUI.gameObject.SetActive(true);
            _reconfirmUI.Init(_controller);
        }
        else
        {
            _warningUI.gameObject.SetActive(true);
            _warningUI.Init(_controller.GetCostByNextLevel());
        }        
    }
}
