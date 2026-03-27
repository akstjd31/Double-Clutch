using TMPro;
using UnityEngine;

public class EndingRollRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _typeText;
    [SerializeField] TextMeshProUGUI _nameText;

    public void Init(string typeKey, string nameKey)
    {
        StringManager stringManager = StringManager.Instance;

        _typeText.text = stringManager.GetString(typeKey);
        _nameText.text = stringManager.GetString(nameKey);

        stringManager.ApplyFont(_nameText);
        stringManager.ApplyFont(_typeText);
    }
}
