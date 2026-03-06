using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InfraUpgradeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _effectDescText;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        
    }
    
    public void Init(Infra infra)
    {
        _title.text = infra.Name;
        _nameText.text = infra.Name;

        _levelText.text = infra.CurrentLevel.ToString();

        var valueStr = TextParser.GetKeys(infra.Desc);
        var fommatted = FormatInfraDescText(infra.Desc, "{" + valueStr[0] + "}", infra.GetValueByLevel());
        _effectDescText.text = fommatted;
    }

    // 중괄호로 되어있는 부분을 처리 및 전체 문자열을 반환 (이 부분은 UI 스크립트에서 작성해야할듯?)
    private string FormatInfraDescText(string desc, string target, int infraEffectValue) => desc.Replace(target, infraEffectValue.ToString());
}
