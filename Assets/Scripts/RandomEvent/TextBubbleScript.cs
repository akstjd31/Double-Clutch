using UnityEngine;
using TMPro;

public class TextBubbleScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _printText;

    public TextMeshProUGUI NameText => _nameText;
    public TextMeshProUGUI PrintText => _printText;

    private void OnEnable()
    {
        _nameText.text = "";
        _printText.text = "";
    }
}
