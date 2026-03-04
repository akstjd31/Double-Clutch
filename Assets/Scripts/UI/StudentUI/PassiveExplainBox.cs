using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveExplainBox : MonoBehaviour
{
    [SerializeField] TMP_Text _skillNameText;
    [SerializeField] TMP_Text _skillDescText;

    public void Init(Player_PassiveData? data)
    {
        if (!data.HasValue) return;

        if (_skillNameText == null || _skillDescText == null) return;

        if (StringManager.Instance == null) return;

        _skillNameText.text = StringManager.Instance.GetString(data.Value.skillName ?? "");
        _skillDescText.text = StringManager.Instance.GetString(data.Value.passiveDesc ?? "");
    }
}
