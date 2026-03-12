using TMPro;
using UnityEngine;

public class PassiveExplainBox : MonoBehaviour
{
    [SerializeField] TMP_Text _skillNameText;
    [SerializeField] TMP_Text _skillDescText;

    Player_PassiveData? _data;

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }

    public void Init(Player_PassiveData? data)
    {
        _data = data;
        Refresh();
    }

    private void Refresh()
    {
        if (!_data.HasValue) return;
        if (_skillNameText == null || _skillDescText == null) return;
        if (StringManager.Instance == null) return;

        _skillNameText.text = StringManager.Instance.GetString(_data.Value.skillName ?? "");
        string originDesc = StringManager.Instance.GetString(_data.Value.passiveDesc ?? "");
        string valueString = string.Empty;

        switch (_data.Value.effectType)
        {
            case effectType.None:
                break;
            case effectType.Rate2pt:
            case effectType.Rate3pt:
            case effectType.RateBlock:
            case effectType.RatePass:
            case effectType.RateSteal:
            case effectType.RateRebound:
            case effectType.MonthGoldUp:
            case effectType.MatchGoldUp:
                valueString = (_data.Value.effectValue * 100).ToString()+"%";
                break;
            default:
                valueString = (_data.Value.effectValue).ToString();
                break;
        }

        string formattedDesc = originDesc.Replace("{effectValue}", valueString);
        _skillDescText.text = formattedDesc;
    }
}
