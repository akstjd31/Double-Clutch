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
        _skillDescText.text = StringManager.Instance.GetString(_data.Value.passiveDesc ?? "");
    }
}
