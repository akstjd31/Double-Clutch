using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveExplainBox : MonoBehaviour
{
    [SerializeField] Text _skillNameText;
    [SerializeField] Text _skillDescText;

    public void Init(Player_PassiveData data)
    {
        _skillNameText.text = StringManager.Instance.GetString(data.skillName);
        _skillDescText.text = StringManager.Instance.GetString(data.passiveDesc);
    }

}
